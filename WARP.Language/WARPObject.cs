using System;
using Interpreter.Abstractions;

namespace WARP.Language {
        public class WARPObject : BaseObject {
                internal static int CurrentRadix = FlexibleNumeralSystem.StandardRadix;

                public WARPObject()
                        : this(0L) { }

                internal WARPObject(char src) : this(new string(new[] { src })) { }

                internal WARPObject(string src) {
                        Source = src;
                        Radix = CurrentRadix;
                }

                internal WARPObject(long src)
                        : this(src.ToString()) { }


                internal bool IsNumeric => FlexibleNumeralSystem.CanParse(Source, Radix);

                private string Source { get; set; }

                private int Radix { get; set; }

                internal long AsNumeric(long defaultValue = 0L) =>
                        FlexibleNumeralSystem.Decode(Source, Radix, defaultValue);

                internal string AsString() => Source ?? string.Empty;

                internal char AsCharacter() => Convert.ToChar(AsNumeric());

                public override object Clone() => new WARPObject { Source = Source, Radix = Radix };

                internal static WARPObject Mutate(string src) => new WARPObject { Source = src, Radix = CurrentRadix };

                public override string ToString() => AsString();
        }
}