//============================================================================================================================================================================
// Copyright (c) 2011-2013 Tony Beveridge
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//============================================================================================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Interpreter.Abstractions {
        public class Interpreter<TSourceType, TExeType> : IInterpreter<TSourceType, TExeType>
                where TSourceType : SourceCode, new() where TExeType : BaseInterpreterStack, new() {
                private const string EOLConfiguration = "retainSourceEOL";

                private readonly List<ITrivialInterpreterBase<TSourceType, TExeType>> Interpreters =
                        new List<ITrivialInterpreterBase<TSourceType, TExeType>>();

                public Interpreter(Assembly ass, string baseConfigName = null) : this(new[] { ass }) {
                }

                public Interpreter(Assembly[] asses, string baseConfigName = null) {
                        DetectInterpreters(asses);
                        State = new InterpreterState().Establish<TSourceType, TExeType>();
                }

                private bool SkipUnknownCommands { get; set; }

                public IEnumerable<Func<InterpreterState, bool>> BreakpointDetectors { get; set; }

                public ReadOnlyCollection<ITrivialInterpreterBase<TSourceType, TExeType>> KnownInterpreters => Interpreters.AsReadOnly();

                private TSourceType SourceCode => State.GetSource<TSourceType>();

                public InterpreterState State { get; protected set; }

                public async Task<InterpreterResult> ExecuteAsync() {
                        var result = InterpreterResult.Complete;
                        try {
                                while (await StepAsync() && result == InterpreterResult.Complete)
                                        if (BreakpointDetectors != null && BreakpointDetectors.Any(f => f(State)))
                                                result = InterpreterResult.BreakpointReached;
                        }
                        catch (Exception ex) {
                                InterpreterEvent?.Invoke(this,
                                        new InterpreterEventArgs<TSourceType, TExeType> {
                                                ActiveInterpreter = this,
                                                ErrorState = true
                                        });
                                throw;
                        }

                        return result;
                }

                public BaseObject Gather() {
                        var interp =
                                Interpreters.FirstOrDefault(tib => tib.Applicable(State));
                        ExecutionSupport.Assert(interp != null || SkipUnknownCommands,
                                $"No interpreter usable; offending character {SourceCode.Current()}");
                        BaseObject result = NullObject.Instance;
                        if (interp == null) {
                                State.BaseSourceCode.Advance();
                        }
                        else {
                                interp.Interpreter = this;
                                result = interp.Gather(State);
                        }

                        return result;
                }

                public bool StepMode { get; set; }

                public Interpreter<TSourceType, TExeType> IgnoreUnknownCommands(bool ignore = true) =>
                        this.Fluently(() => SkipUnknownCommands = ignore);

                public event EventHandler<InterpreterEventArgs<TSourceType, TExeType>> InterpreterEvent;

                public void Accept(string fileName) {
                        ExecutionSupport.Assert(File.Exists(fileName),
                                string.Format("File {0} does not exist", fileName));
                        AcceptSource(File.ReadAllLines(fileName));
                }

                public void AcceptSource(string[] src, bool? retain = null) {
                        var retainEOL = retain ?? ConfigurationSupport.ConfigurationFor(EOLConfiguration, true);
                        State.GetSource<TSourceType>().Content = src
                                .Select(s => string.Concat(s, retainEOL ? Environment.NewLine : string.Empty)).ToList();
                }

                public async Task<bool> StepAsync() {
                        var possible = State.GetSource<TSourceType>().More();
                        if (possible) {
                                var obj = Gather();
                                if (obj != null)
                                        await obj.ApplyAsync(State);
                        }
                        return possible;
                }

                public Interpreter<TSourceType, TExeType> Register(
                        EventHandler<InterpreterEventArgs<TSourceType, TExeType>> handler) {
                        InterpreterEvent += handler;
                        return this;
                }

                private void DetectInterpreters(Assembly[] asses) =>
                        Interpreters.AddRange(
                                asses
                                        .SelectMany(a => a.GetTypes()) // Slow, but not really an issue
                                        .Where(t =>
                                                t.GetInterface(typeof(ITrivialInterpreterBase<TSourceType, TExeType>).Name) !=
                                                null && !t.IsAbstract)
                                        .Select(t =>
                                                Activator.CreateInstance(t) as ITrivialInterpreterBase<TSourceType, TExeType>));
        }
}