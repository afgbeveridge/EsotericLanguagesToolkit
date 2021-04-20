using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Eso.API.Services {
        public class LocalIOWrapper : IOWrapper {
                internal LocalIOWrapper(WebSocket socket, CancellationTokenSource local) {
                        Channel = socket;
                        LocalToken = local;
                }

                private WebSocket Channel { get; }
                private CancellationTokenSource LocalToken { get; }

                public async Task<char> ReadCharacterAsync() => (await Receive()).First();

                public async Task<string> ReadStringAsync(string defaultIfEmpty = "") =>
                        await Receive() ?? defaultIfEmpty;

                public async Task WriteAsync(string src) => await Send(src);

                public async Task WriteAsync(char c) => await Send(new string(c, 1));

                private async Task Send(string content) {
                        var outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(content));
                        // TODO: Detect closure here. Need to rmq
                        await Channel.SendAsync(outputBuffer, WebSocketMessageType.Text, true, LocalToken.Token);
                }

                internal async Task<string> Receive(bool promptRemote = true) {
                        if (promptRemote)
                                await Send("\t");
                        var buffer = new byte[2048];
                        var response = await Channel.ReceiveAsync(new ArraySegment<byte>(buffer), LocalToken.Token);
                        // TODO: Detect closure here. Need to rmq
                        // TODO: Ensure complete message is received
                        return Encoding.ASCII.GetString(buffer, 0, response.Count);
                }
        }
}