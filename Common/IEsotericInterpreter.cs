namespace Common {
        public interface IEsotericInterpreter {
                string Language { get; }

                string Summary { get; }

                string Url { get; }

                void Interpret(IOWrapper wrapper, string[] src);
        }
}