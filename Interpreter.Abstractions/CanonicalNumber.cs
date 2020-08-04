using System;

namespace Interpreter.Abstractions {
        public class CanonicalNumber : BaseObject {
                private const string FalseValue = "falseValue";
                private const string TrueValue = "trueValue";
                protected static readonly int ConfiguredFalse = ConfigurationSupport.ConfigurationFor(FalseValue, 0);
                protected static readonly int ConfiguredTrue = ConfigurationSupport.ConfigurationFor(TrueValue, -1);

                public CanonicalNumber() : this(0) { }

                public CanonicalNumber(string val) : this(int.Parse(val)) { }

                public CanonicalNumber(int num) => Value = num;

                public int Value { get; set; }

                public override BaseObject Accept(string src) {
                        Value = int.Parse(src);
                        return this;
                }

                public CanonicalNumber Invert() => new CanonicalNumber(Value * -1);

                public override object Clone() => new CanonicalNumber(Value);

                public override bool Equals(object obj) =>
                        obj is CanonicalNumber ? ((CanonicalNumber) obj).Value == Value : false;

                public override int GetHashCode() => Value;

                public override string ToString() => GetType().Name + " == " + Value;

                public static CanonicalNumber operator ==(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value == rhs.Value);

                public static CanonicalNumber operator !=(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value != rhs.Value);

                public static CanonicalNumber operator <(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value < rhs.Value);

                public static CanonicalNumber operator <=(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value <= rhs.Value);

                public static CanonicalNumber operator >=(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(!(lhs < rhs));

                public static CanonicalNumber operator >(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value > rhs.Value);

                public static CanonicalNumber operator +(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value + rhs.Value);

                public static CanonicalNumber operator -(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value - rhs.Value);

                public static CanonicalNumber operator *(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value * rhs.Value);

                public static CanonicalNumber operator /(CanonicalNumber lhs, CanonicalNumber rhs) {
                        double res = lhs.Value / rhs.Value;
                        return new CanonicalBoolean((int) (res < 0 ? Math.Ceiling(res) : Math.Floor(res)));
                }

                public static CanonicalNumber operator &(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value & rhs.Value);

                public static CanonicalNumber operator |(CanonicalNumber lhs, CanonicalNumber rhs) =>
                        new CanonicalBoolean(lhs.Value | rhs.Value);

                public CanonicalBoolean Negate() => new CanonicalBoolean(Value == ConfiguredFalse);

                public static implicit operator bool(CanonicalNumber obj) => obj.Value != ConfiguredFalse;

                public static bool operator true(CanonicalNumber obj) => obj.Value != ConfiguredFalse;

                public static bool operator false(CanonicalNumber obj) => obj.Value == ConfiguredFalse;
        }
}