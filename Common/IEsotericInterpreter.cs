using System.Threading.Tasks;

namespace Common {
        public interface IEsotericInterpreter {
                string Language { get; }

                string Summary { get; }

                string Url { get; }

                Task InterpretAsync(IOWrapper wrapper, string[] src);
        }
}