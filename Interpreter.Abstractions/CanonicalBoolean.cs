namespace Interpreter.Abstractions {
        public class CanonicalBoolean : CanonicalNumber {
                public CanonicalBoolean(bool val)
                        : this(val ? ConfiguredTrue : ConfiguredFalse) { }

                public CanonicalBoolean(int num)
                        : base(num) { }

                public static CanonicalBoolean True => new CanonicalBoolean(true);

                public static CanonicalBoolean False => new CanonicalBoolean(false);

                public override string ToString() =>
                        GetType().Name + " == " + Value + "(" + (Value != ConfiguredFalse) + ")";

                public override object Clone() => new CanonicalBoolean(Value);

                public static implicit operator bool(CanonicalBoolean obj) => obj.Value != ConfiguredFalse;
        }
}