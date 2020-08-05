using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Interpreter.Abstractions {
        public class CommandLineExecutor<TSourceType, TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                // Primitive validation

                private static readonly string[] SupportedDebugOptions = { null, string.Empty, "c", "n" };

                private readonly Dictionary<string, OptionBehaviour<TSourceType, TExeType>> OptionsAndConstraints =
                        new Dictionary<string, OptionBehaviour<TSourceType, TExeType>> {
                                {
                                        "-s",
                                        new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.None,
                                                Help = "-s  activates step mode",
                                                Effect = (cle, interp, args) => interp.StepMode = true
                                        }
                                }, {
                                        "-d",
                                        new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.None,
                                                Help = "-d  activate debug file generation",
                                                Effect = (cle, interp, args) => ExecutionSupport.DebugMode = true
                                        }
                                }, {
                                        "-t",
                                        new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.None,
                                                Help = "-t  show stack in step mode",
                                                Effect = (cle, interp, args) => cle.ShowStackWhenStepping = true
                                        }
                                }, {
                                        "-tr", new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.None,
                                                Help = "-tr  show deep stack in step mode",
                                                Effect = (cle, interp, args) => {
                                                        cle.ShowStackWhenStepping = true;
                                                        cle.ShowStackRecursivelyWhenStepping =
                                                                StackDumpDirective.Recursive;
                                                }
                                        }
                                }, {
                                        "-bps", new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.Binary,
                                                Help = "-bp break at source position x, y",
                                                Effect = (cle, interp, args) => {
                                                        interp.BreakpointDetectors =
                                                                new List<Func<InterpreterState, bool>> {
                                                                        s => s.BaseSourceCode.SourcePosition.X ==
                                                                             int.Parse(args.First()) &&
                                                                             s.BaseSourceCode.SourcePosition.Y ==
                                                                             int.Parse(args[1])
                                                                };
                                                },
                                                Transient = true
                                        }
                                }, {
                                        "-esc", new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.None,
                                                Help = "-esc  enable source code seek cache",
                                                Effect = (cle, interp, args) =>
                                                        interp.State.BaseSourceCode.CachingEnabled = true
                                        }
                                }, {
                                        "-n",
                                        new OptionBehaviour<TSourceType, TExeType> {
                                                ArgumentCount = OptionCount.None,
                                                Help = "-n  suppress interpreter startup message",
                                                Effect = (cle, interp, args) => cle.SuppressStartupMessage = true
                                        }
                                }
                        };

                private bool SuppressStartupMessage { get; set; }

                private Interpreter<TSourceType, TExeType> Interpreter { get; set; }

                private bool ShowStackWhenStepping { get; set; }

                private StackDumpDirective ShowStackRecursivelyWhenStepping { get; set; }

                public void Execute(Assembly ass, string startupMessage, string[] args,
                        Action<Interpreter<TSourceType, TExeType>> preExecution = null) {
                        try {
                                ExecutionSupport.Assert(args.Any(),
                                        string.Concat("Usage: .exe [options] {source file}+", OptionsHelp()));
                                if (!ConsoleEx.IsInputRedirected) {
                                        Console.TreatControlCAsInput = false;
                                        Console.CancelKeyPress += (sender, e) => {
                                                Console.WriteLine("Execution aborted by user....");
                                        };
                                }

                                // Use a simple for loop as options may cause 'argument' jumping when being gathered
                                for (var i = 0; i < args.Length; i++) {
                                        var cur = args[i];
                                        if (IsOption(cur)) {
                                                i = ExamineOption(cur, args, i);
                                        }
                                        else {
                                                Interpreter =
                                                        new Interpreter<TSourceType, TExeType>(ass).Register(
                                                                FormatInterpreterState);
                                                ApplyActiveOptions();
                                                if (i == 0 && !SuppressStartupMessage)
                                                        Console.WriteLine(startupMessage);

                                                ExecutionSupport.Prepare(cur);
                                                preExecution?.Invoke(Interpreter);

                                                Interpreter.Accept(cur);
                                                Process();
                                        }
                                }
                        }
                        catch (Exception ex) {
                                ConsoleHighlighter.Display(string.Concat("Exception during execution -> ", ex.Message),
                                        ConsoleColor.DarkYellow);
                                Console.WriteLine(string.Concat("Stack trace: ", Environment.NewLine, ex.ToString()));
                                if (Interpreter != null && Interpreter.State != null)
                                        Console.WriteLine(string.Concat("Current state: ", Environment.NewLine,
                                                Interpreter.State.BaseExecutionEnvironment.ToString(StackDumpDirective
                                                        .Recursive)));
                        }
                        finally {
                                ExecutionSupport.DebugMode = false;
                                ExecutionSupport.UnPrepare();
                        }
                }

                private void Process() {
                        var result = InterpreterResult.InFlight;
                        while (result != InterpreterResult.Complete)
                                if (Interpreter.StepMode) {
                                        StepInto();
                                }
                                else {
                                        result = Interpreter.Execute();
                                        if (result == InterpreterResult.BreakpointReached) Interpreter.StepMode = true;
                                }
                }

                private void StepInto() {
                        Interpreter.State.GetSource<SourceCode>().SourceEvent += SourceEventSubscriber;
                        Step();
                }

                // Has to be a method and not an anonymous subscriber as electing to "run" in step mode should unsubscribe, and unsubscription cannot be done reliably with anon delegates 
                private void SourceEventSubscriber(object sender, SourceCodeEventArgs<SourceCode> arguments) {
                        MarkSource(arguments.Source);
                        if (ShowStackWhenStepping)
                                Console.WriteLine(
                                        Interpreter.State.BaseExecutionEnvironment.ToString(
                                                ShowStackRecursivelyWhenStepping));
                }

                private string OptionsHelp() =>
                        string.Concat(Environment.NewLine,
                                string.Join(Environment.NewLine, OptionsAndConstraints.Select(kvp => kvp.Value.Help)));

                private bool IsOption(string s) => OptionsAndConstraints.ContainsKey(s);

                private int ExamineOption(string key, string[] args, int index) {
                        var option = OptionsAndConstraints[key];
                        option.Active = !option.Active;
                        ExecutionSupport.Assert(index + 1 + option.ArgumentCount < args.Length,
                                string.Concat("Not enough arguments for ", key));
                        if (option.ArgumentCount != OptionCount.None)
                                option.Arguments = args.ToList().GetRange(index + 1, option.ArgumentCount).ToArray();

                        return index + option.ArgumentCount;
                }

                private void ApplyActiveOptions() =>
                        OptionsAndConstraints.Where(kvp => kvp.Value.Active).ToList().ForEach(kvp => {
                                kvp.Value.Effect(this, Interpreter, kvp.Value.Arguments);
                                kvp.Value.Active = !kvp.Value.Transient;
                        });

                private void Step() {
                        while (Interpreter.Step() && Interact()) ;

                        Interpreter.StepMode = false;
                }

                private bool Continue(bool removeBreakpoints) {
                        Interpreter.State.GetSource<SourceCode>().SourceEvent -= SourceEventSubscriber;
                        Interpreter.StepMode = false;
                        Interpreter.BreakpointDetectors = removeBreakpoints ? null : Interpreter.BreakpointDetectors;
                        return false;
                }

                private void FormatInterpreterState(object sender, InterpreterEventArgs<TSourceType, TExeType> e) {
                        if (e.ErrorState) Dump(e.ActiveInterpreter);
                }

                private void Dump(Interpreter<TSourceType, TExeType> current) {
                        var state = current.State.GetSource<SourceCode>();
                        ConsoleHighlighter.Display(Environment.NewLine + "Interpreter error" + Environment.NewLine +
                                                   "Total source size: "); // + state.Content.Length);
                        MarkSource(state);
                }

                private void MarkSource(SourceCode source) {
                        var idx = 0;
                        var marked = false;
                        while (!marked && idx < source.Content.Count) {
                                if (idx != source.SourcePosition.Y) Console.WriteLine(source.Content[idx]);

                                if (idx == source.SourcePosition.Y) {
                                        var line = source.Content[idx];
                                        Console.Write(line.Substring(0, source.SourcePosition.X));
                                        ConsoleHighlighter.Display(new string(new[] { line[source.SourcePosition.X] }),
                                                ConsoleColor.Yellow, ConsoleColor.DarkRed, false);
                                        Console.WriteLine(line.Substring(source.SourcePosition.X + 1));
                                        ConsoleHighlighter.Display(new string(' ', source.SourcePosition.X) + "^",
                                                ConsoleColor.Cyan);
                                        marked = true;
                                }

                                idx++;
                        }

                        Console.WriteLine(string.Concat("Source position: (", source.SourcePosition.X, ",",
                                source.SourcePosition.Y, ")"));
                }

                private bool Interact() {
                        Console.Write(string.Concat(
                                "[c] = continue (ignore breakpoints), [n] = execute to next breakpoint",
                                Environment.NewLine, "[ENTER] to step..."));
                        string cmd = null;
                        do {
                                string.IsNullOrEmpty(cmd).IfFalse(() =>
                                        Console.WriteLine(string.Concat("? => Don't understand ", cmd)));
                                cmd = (Console.ReadLine() ?? string.Empty).Trim();
                        } while (!SupportedDebugOptions.Contains(cmd));

                        return string.IsNullOrEmpty(cmd) ? true : Continue(cmd == "c");
                }
        }
}