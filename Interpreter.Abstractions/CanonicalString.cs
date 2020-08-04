namespace Interpreter.Abstractions {
        public class CanonicalString : BaseObject {
                public CanonicalString() { }

                public CanonicalString(string src) => Source = src;

                private string Source { get; set; }

                public override BaseObject Accept(string src) {
                        Source = src;
                        return this;
                }

                public override string ToString() => Source;
        }
}