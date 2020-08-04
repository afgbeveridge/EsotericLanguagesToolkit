using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Eso.API.Services {
        public interface IWebSocketHandler {
                Task<IWebSocketHandler> Host(WebSocket socket);
                Task Process();
        }
}