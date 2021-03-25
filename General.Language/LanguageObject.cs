using System;
using Interpreter.Abstractions;

namespace General.Language {
        public class LanguageObject : BaseObject {

                public LanguageObject()
                        : this(FlexibleNumeralSystem.StandardRadix, 0L) { }

                public LanguageObject(int radix)
                        : this(radix, 0L) { }

                internal LanguageObject(int radix, char src) : this(radix, new string(new[] { src })) { }

                internal LanguageObject(int radix, string src) {
                        Source = src;
                        Radix = radix;
                }

                internal LanguageObject(int radix, long src)
                        : this(radix, src.ToString()) { }


                public bool IsNumeric => FlexibleNumeralSystem.CanParse(Source, Radix);

                private string Source { get; }

                private int Radix { get; }

                public long AsNumeric(long defaultValue = 0L) => FlexibleNumeralSystem.Decode(Source, Radix, defaultValue);

                public string AsString() => Source ?? string.Empty;

                public char AsCharacter() => Convert.ToChar(AsNumeric());

                public override object Clone() => new LanguageObject(Radix, Source);

                internal static LanguageObject Mutate(int radix, string src) => new LanguageObject(radix, src);

                public override string ToString() => AsString();
        }
}