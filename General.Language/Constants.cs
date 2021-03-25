using System.Collections.Generic;

namespace General.Language {
        public static class Constants {

                public const string RASName = "__RAS";
                public const string CurrentBase = "__CIB";
                public const string Builder = "__BLDR";
                public const string CurrentRadix = "__RADIX";

                public static Dictionary<KnownConcept, string> DefaultKeywordBindings { get; } = new Dictionary<KnownConcept, string> {
                        [KnownConcept.Comparison] = ":",
                        [KnownConcept.Pop] = "!",
                        [KnownConcept.Treat] = "%",
                        [KnownConcept.Untreat] = "|",
                        [KnownConcept.Push] = "*",
                        [KnownConcept.PopAndPush] = "]",
                        [KnownConcept.DuplicateTOS] = ";",
                        [KnownConcept.RotateStacks] = "'",
                        [KnownConcept.RetrieveFromRAS] = "{",
                        [KnownConcept.PlaceInRAS] = "}",
                        [KnownConcept.Addition] = ">",
                        [KnownConcept.Subtraction] = "<",
                        [KnownConcept.Multiplication] = "&",
                        [KnownConcept.Division] = "$",
                        [KnownConcept.Modulo] = "#",
                        [KnownConcept.Jump] = "^",
                        [KnownConcept.Label] = "@",
                        [KnownConcept.Assignment] = "=",
                        [KnownConcept.ConditionalExecution] = "?",
                        [KnownConcept.RadixSwitch] = "+",
                        [KnownConcept.OutputCharacter] = "(",
                        [KnownConcept.OutputNativeForm] = ")",
                        [KnownConcept.Input] = ",",
                        [KnownConcept.CurrentStack] = "_",
                        [KnownConcept.Quine] = "~",
                        [KnownConcept.Period] = "."
                };
                
        }
}