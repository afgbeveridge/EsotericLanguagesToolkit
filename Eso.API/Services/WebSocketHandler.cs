using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Eso.API.Services {
        public class WebSocketHandler : IWebSocketHandler {
                public WebSocketHandler(IPluginService pluginService) => PluginHandler = pluginService;

                private WebSocket Channel { get; set; }
                private CancellationTokenSource LocalToken { get; set; }
                private IPluginService PluginHandler { get; }
                private string Language { get; set; }

                public Task<IWebSocketHandler> Host(WebSocket socket) {
                        Channel = socket;
                        LocalToken = new CancellationTokenSource();
                        var token = LocalToken.Token;
                        return Task.FromResult((IWebSocketHandler) this);
                }

                public async Task Process() {
                        var wrapper = new LocalIOWrapper(Channel, LocalToken);
                        var bundle = await GetLanguageAndSource(wrapper);
                        var processor = LocateProcessor(bundle.Language);
                        processor.Interpret(wrapper, new[] { bundle.Source });
                        await Channel.CloseAsync(WebSocketCloseStatus.NormalClosure, "Execution complete",
                                LocalToken.Token);
                }

                private async Task<SourceBundle> GetLanguageAndSource(LocalIOWrapper wrapper) {
                        var src = await wrapper.Receive(false);
                        // TODO: Assert
                        var end = src.IndexOf(src.First(), 1);
                        return new SourceBundle {
                                Language = src.Substring(1, end - 1),
                                Source = src.Substring(end + 1)
                        };
                }

                private IEsotericInterpreter LocateProcessor(string language) => PluginHandler.InterpreterFor(language);

                private class SourceBundle {
                        internal string Language { get; set; }
                        internal string Source { get; set; }
                }
        }
}