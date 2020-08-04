using System.Threading.Tasks;

namespace Common {
        public interface IOWrapper {
                Task<char> ReadCharacterAsync();
                Task<string> ReadStringAsync(string defaultIfEmpty = "");
                Task WriteAsync(string src);
                Task WriteAsync(char c);
        }
}