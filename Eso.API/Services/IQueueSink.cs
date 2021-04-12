
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Eso.API.Services {
        public interface IQueueSink {
                IQueueSink UsingConfiguration(IConfiguration cfg, string name = null);

                Task PublishAsync(string queue, string msg, string exchange = null);

        }

}
