using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace Eso.API.Services {
        public class SelfContainedWrapper : IOWrapper {
                internal SelfContainedWrapper(IEnumerable<string> input) =>
                        Input = input?.ToList() ?? new List<string>();

                internal string Sink { get; set; } = string.Empty;

                private List<string> Input { get; }

                private int CurrentLine { get; set; }

                private int CurrentIndex { get; set; }

                public Task<char> ReadCharacterAsync() {
                        var cur = Input[CurrentLine];
                        if (cur.Length <= CurrentIndex) {
                                CurrentIndex = 0;
                                cur = Input[CurrentLine++];
                        }

                        return Task.FromResult(cur[CurrentIndex++]);
                }

                public Task<string> ReadStringAsync(string defaultIfEmpty = "") {
                        var cur = Input[CurrentLine++];
                        return Task.FromResult(cur);
                }

                public Task WriteAsync(string src) {
                        Sink = $"{Sink}{src}";
                        return Task.CompletedTask;
                }

                public Task WriteAsync(char c) {
                        Sink = $"{Sink}{c}";
                        return Task.CompletedTask;
                }
        }
}