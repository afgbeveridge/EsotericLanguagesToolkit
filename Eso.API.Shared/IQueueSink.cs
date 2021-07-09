
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Eso.API.Shared {
        public interface IQueueSink {
                IQueueSink UsingConfiguration(IConfiguration cfg, string name = null);

                Task PublishToExchangeAsync(string exchange, string msg);

                Task PublishToQueueAsync(string queue, string msg);

        }

}
