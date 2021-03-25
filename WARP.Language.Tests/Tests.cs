using System;
using System.Collections.Generic;
using Interpreter.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using General.Language;
using GeneralConstants = General.Language.Constants;
using System.Threading.Tasks;

namespace WARP.Language.Tests {
        [TestClass]
        public class LanguageTest {
                /// <summary>
                ///         Gets or sets the test context which provides
                ///         information about and functionality for the current test run.
                /// </summary>
                public TestContext TestContext { get; set; }

                private InterpreterState State { get; set; }

                private General.Language.CommandBuilder Builder { get; set; }

                [TestInitialize]
                public void Initialize() {
                        Builder = new CommandBuilder().WithBindings(Constants.DefaultKeywordBindings).Initialize();
                        State = new InterpreterState().Establish<SimpleSourceCode, PropertyBasedExecutionEnvironment>();
                        Stack().Reset();
                        State.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()
                                .ScratchPad[GeneralConstants.CurrentRadix] = FlexibleNumeralSystem.StandardRadix;
                        State.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>().ScratchPad[GeneralConstants.RASName]
                                = new RandomAccessStack<LanguageObject> { MaximumSize = 50000 };
                        State.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>().OnUnknownKey =
                                env => new LanguageObject();
                        State.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()
                                .ScratchPad[GeneralConstants.CurrentBase] = new ConsoleIOWrapper();
                }

                private LanguageTest EstablishProgramSource(string code) {
                        State.GetSource<SimpleSourceCode>().Content = new List<string> { code };
                        return this;
                }

                private LanguageObject Dereference(string key) =>
                        State.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()[key] as LanguageObject;

                private LanguageObject TOS() => Stack().Pop<LanguageObject>();

                private PropertyBasedExecutionEnvironment Stack() => State.Stack<PropertyBasedExecutionEnvironment>();

                private async Task<LanguageTest> Parse() {
                        while (State.GetSource<SimpleSourceCode>().More())
                                await Builder.Gather(State).ApplyAsync(State);
                        return this;
                }
                private async Task<LanguageTest> EstablishSourceAndParse(string code) {
                        EstablishProgramSource(code);
                        while (State.GetSource<SimpleSourceCode>().More())
                                await Builder.Gather(State).ApplyAsync(State);
                        return this;
                }

                private async Task<LanguageObject> ParseAndDereference(string key) => (await Parse()).Dereference(key);

                private async Task<LanguageObject> ParseAndReturnTOS() => (await Parse()).TOS();

                [TestMethod]
                public void SupportsAssign() {
                        EstablishProgramSource("=");
                        Assert.IsTrue(Builder.Applicable(State), "Assign should be supported");
                }

                [TestMethod]
                public async Task EmptyProgram() => await EstablishSourceAndParse(string.Empty);

                [TestMethod]
                public async Task InvalidProgram() => await EstablishSourceAndParse("2");

                [TestMethod]
                public async Task SimpleAssign() {
                        var x = await EstablishProgramSource("=ab1").ParseAndDereference("ab");
                        Assert.AreEqual("1", x.AsString());
                        Assert.IsTrue(x.IsNumeric);
                }

                [TestMethod]
                public async Task ObjectAssign() {
                        var x = await EstablishProgramSource("=ab1=bcab").ParseAndDereference("bc");
                        Assert.AreEqual("1", x.AsString());
                }

                [TestMethod]
                public async Task StringAssign() {
                        var x = await EstablishProgramSource("=ab\"Content\"").ParseAndDereference("ab");
                        Assert.AreEqual("Content", x.AsString());
                }

                [TestMethod]
                public async Task SimplePush() {
                        var x = await EstablishProgramSource("*\"Content\"").ParseAndReturnTOS();
                        Assert.AreEqual("Content", x.AsString());
                }

                [TestMethod]
                public async Task ObjectPush() {
                        var x = await EstablishProgramSource("=tt\"Content\"*tt").ParseAndReturnTOS();
                        Assert.AreEqual("Content", x.AsString());
                }

                [TestMethod]
                public async Task PopAssign() {
                        var x = await EstablishProgramSource("*17=ab!").ParseAndDereference("ab");
                        Assert.AreEqual("17", x.AsString());
                }

                [TestMethod]
                public async Task SimpleAddition() {
                        var x = await EstablishProgramSource("=ab5>ab2").ParseAndDereference("ab");
                        Assert.AreEqual("7", x.AsString());
                }

                [TestMethod]
                public async Task ObjectAddition() {
                        var x = await EstablishProgramSource("=ab5=xx2>abxx").ParseAndDereference("ab");
                        Assert.AreEqual("7", x.AsString());
                }

                [TestMethod]
                public async Task CoercedAddition() {
                        var x = await EstablishProgramSource("=ab5=xx\"2\">abxx").ParseAndDereference("ab");
                        Assert.AreEqual("7", x.AsString());
                }

                [TestMethod]
                public async Task StackAddition() =>
                        Assert.AreEqual(6, (await EstablishProgramSource("*5>!1").ParseAndReturnTOS()).AsNumeric());

                [TestMethod]
                public async Task SimpleSubtraction() {
                        var x = await EstablishProgramSource("=ab5<ab2").ParseAndDereference("ab");
                        Assert.AreEqual("3", x.AsString());
                }

                [TestMethod]
                public async Task ObjectSubtraction() {
                        var x = await EstablishProgramSource("=ab5=xx2<abxx").ParseAndDereference("ab");
                        Assert.AreEqual("3", x.AsString());
                }

                [TestMethod]
                public async Task CoercedSubtraction() {
                        var x = await EstablishProgramSource("=ab5=xx\"2\"<abxx").ParseAndDereference("ab");
                        Assert.AreEqual("3", x.AsString());
                }

                [TestMethod]
                public async Task SimpleMultiplication() {
                        var x = await EstablishProgramSource("=ab5&ab2").ParseAndDereference("ab");
                        Assert.AreEqual("A", x.AsString());
                }

                [TestMethod]
                public async Task ObjectMultiplication() {
                        var x = await EstablishProgramSource("=ab5=xx2&abxx").ParseAndDereference("ab");
                        Assert.AreEqual("A", x.AsString());
                }

                [TestMethod]
                public async Task CoercedMultiplication() {
                        var x = await EstablishProgramSource("=ab5=xx\"2\"&abxx").ParseAndDereference("ab");
                        Assert.AreEqual("A", x.AsString());
                }

                [TestMethod]
                public async Task SimpleDivision() {
                        var x = await EstablishProgramSource("=ab5$ab2").ParseAndDereference("ab");
                        Assert.AreEqual("2", x.AsString());
                }

                [TestMethod]
                public async Task ObjectDivision() {
                        var x = await EstablishProgramSource("=ab5=xx2$abxx").ParseAndDereference("ab");
                        Assert.AreEqual("2", x.AsString());
                }

                [TestMethod]
                public async Task CoercedDivision() {
                        var x = await EstablishProgramSource("=ab5=xx\"2\"$abxx").ParseAndDereference("ab");
                        Assert.AreEqual("2", x.AsString());
                }

                [TestMethod]
                public async Task SimpleModulo() {
                        var x = await EstablishProgramSource("=ab5#ab2").ParseAndDereference("ab");
                        Assert.AreEqual("1", x.AsString());
                }

                [TestMethod]
                public async Task ObjectModulo() {
                        var x = await EstablishProgramSource("=ab5=xx2#abxx").ParseAndDereference("ab");
                        Assert.AreEqual("1", x.AsString());
                }

                [TestMethod]
                public async Task CoercedModulo() {
                        var x = await EstablishProgramSource("=ab5=xx\"2\"#abxx").ParseAndDereference("ab");
                        Assert.AreEqual("1", x.AsString());
                }

                [TestMethod]
                public async Task DuplicateStackItem() {
                        await EstablishProgramSource("*1;").Parse();
                        Assert.IsTrue(Stack().Size == 2);
                        Assert.AreEqual(TOS().AsString(), TOS().AsString());
                }

                [TestMethod]
                public async Task DecimalRadix() {
                        EstablishProgramSource("+A=ab105");
                        Assert.AreEqual(105, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task HexatrigesimalRadix() {
                        EstablishProgramSource("=ab255S");
                        Assert.AreEqual(100000, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task BinaryRadix() {
                        EstablishProgramSource("+2=ab1010");
                        Assert.AreEqual(10, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task OctalRadix() {
                        EstablishProgramSource("+8=ab377");
                        Assert.AreEqual(255, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task Base11Radix() {
                        EstablishProgramSource("+B=ab120");
                        Assert.AreEqual(143, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task NegativeNumberTest() {
                        EstablishProgramSource("+A=ab-120");
                        Assert.AreEqual(-120, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task RadixSwap() {
                        EstablishProgramSource("+A=ab37+36=cd11");
                        Assert.AreEqual((await ParseAndDereference("cd")).AsNumeric(), (await ParseAndDereference("ab")).AsNumeric());
                }

                // Invalid numerics should return a default value of zero long
                [TestMethod]
                public async Task InvalidNumeric() {
                        EstablishProgramSource("+A=abA11");
                        Assert.AreEqual(0, (await ParseAndDereference("ab")).AsNumeric());
                }

                [TestMethod]
                public async Task PopAndPushCommand() {
                        await EstablishProgramSource("*2]3").Parse();
                        Assert.IsTrue(Stack().Size == 1);
                        Assert.AreEqual(3, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task SimpleLoop() {
                        EstablishProgramSource("=ab2=dc0@a>dc2<ab1^aba");
                        Assert.AreEqual(4, (await ParseAndDereference("dc")).AsNumeric());
                }

                [TestMethod]
                public async Task NestedLoop() {
                        EstablishProgramSource("=ab2=dc0@a>dc2<ab1=xx2@b&dc2<xx1^xxb^aba");
                        Assert.AreEqual(40, (await ParseAndDereference("dc")).AsNumeric());
                }

                [TestMethod]
                [ExpectedException(typeof(ApplicationException))]
                public async Task MissingLabelTest() => await EstablishProgramSource("=ab2@a<ab1^abz").Parse();

                [TestMethod]
                public async Task LoopOnStackCondition() {
                        EstablishProgramSource("*1*2*3=dc0@a>dc!^_a");
                        Assert.AreEqual(6, (await ParseAndDereference("dc")).AsNumeric());
                }

                [TestMethod]
                public async Task UnconditionalJump() {
                        EstablishProgramSource("=dc0^.p>dc1@p");
                        Assert.AreEqual(0, (await ParseAndDereference("dc")).AsNumeric());
                }

                [TestMethod]
                public async Task SimpleEqualComparison() {
                        await EstablishProgramSource(":7:7").Parse();
                        Assert.AreEqual(0, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task SimpleGreaterThanComparison() {
                        await EstablishProgramSource(":8:7").Parse();
                        Assert.AreEqual(1, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task SimpleLessThanComparison() {
                        await EstablishProgramSource(":6:7").Parse();
                        Assert.AreEqual(-1, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task SimpleIfBranch() {
                        EstablishProgramSource(":7:7?0?=yy5");
                        Assert.AreEqual(5, (await ParseAndDereference("yy")).AsNumeric());
                }

                [TestMethod]
                public async Task UnexecutedIfBranch() {
                        EstablishProgramSource("=yy0:6:7?0?=yy5");
                        Assert.AreEqual(0, (await ParseAndDereference("yy")).AsNumeric());
                }

                [TestMethod]
                public async Task SimpleRASAccess() {
                        await EstablishProgramSource("*3}18{18").Parse();
                        Assert.AreEqual(3, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task ExpectInitializedRAS() {
                        await EstablishProgramSource("{18").Parse();
                        Assert.AreEqual(0, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task EnsureRASGrows() {
                        await EstablishProgramSource("{255S").Parse();
                        Assert.AreEqual(0, TOS().AsNumeric());
                }

                [TestMethod]
                public async Task SimpleTreatment() {
                        var content = "A string";
                        await EstablishProgramSource(string.Concat("=st\"", content, "\"%st")).Parse();
                        Assert.AreEqual(content.Length, Stack().Size);
                }

                [TestMethod]
                public async Task SimpleUnTreatment() {
                        await EstablishProgramSource("=st\"A string\"%st|").Parse();
                        Assert.AreEqual(0, Stack().Size);
                }

                [TestMethod]
                public async Task UntreatmentTest() {
                        await EstablishProgramSource("*\"p\"=st\"A string\"%st=tr!|=ts!").Parse();
                        Assert.AreEqual("A", Dereference("tr").AsString());
                        Assert.AreEqual("p", Dereference("ts").AsString());
                }

                [TestMethod]
                public async Task RotationTest() {
                        await EstablishProgramSource("*\"p\"=st\"q\"%st'=aa!'").Parse();
                        Assert.AreEqual("p", Dereference("aa").AsString());
                        Assert.AreEqual("q", TOS().AsString());
                }

                [TestMethod]
                public async Task AutoCreationTest() {
                        await EstablishProgramSource(">vv1").Parse();
                        Assert.AreEqual(1, Dereference("vv").AsNumeric());
                }
        }
}