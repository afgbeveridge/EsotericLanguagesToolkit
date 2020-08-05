using System;
using Interpreter.Abstractions;

namespace WARP.Language {
        public class WARPObject : BaseObject {

                public WARPObject()
                        : this(FlexibleNumeralSystem.StandardRadix, 0L) { }

                public WARPObject(int radix)
                        : this(radix, 0L) { }

                internal WARPObject(int radix, char src) : this(radix, new string(new[] { src })) { }

                internal WARPObject(int radix, string src) {
                        Source = src;
                        Radix = radix;
                }

                internal WARPObject(int radix, long src)
                        : this(radix, src.ToString()) { }


                internal bool IsNumeric => FlexibleNumeralSystem.CanParse(Source, Radix);

                private string Source { get; }

                private int Radix { get; }

                internal long AsNumeric(long defaultValue = 0L) => FlexibleNumeralSystem.Decode(Source, Radix, defaultValue);

                internal string AsString() => Source ?? string.Empty;

                internal char AsCharacter() => Convert.ToChar(AsNumeric());

                public override object Clone() => new WARPObject(Radix, Source);

                internal static WARPObject Mutate(int radix, string src) => new WARPObject(radix, src);

                public override string ToString() => AsString();
        }
}