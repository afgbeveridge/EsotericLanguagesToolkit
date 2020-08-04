using System;
using System.Threading.Tasks;
using Common;

namespace Interpreter.Abstractions {
        public class ConsoleIOWrapper : IOWrapper {
                public Task<char> ReadCharacterAsync() => Task.FromResult(Convert.ToChar(Console.Read()));

                public Task<string> ReadStringAsync(string defaultIfEmpty = "") =>
                        Task.FromResult(Console.ReadLine() ?? defaultIfEmpty);

                public Task WriteAsync(string src) {
                        Console.Write(src);
                        return Task.CompletedTask;
                }

                public Task WriteAsync(char c) {
                        Console.Write(c);
                        return Task.CompletedTask;
                }
        }
}