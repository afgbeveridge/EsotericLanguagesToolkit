using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Eso.API.Services {
        public class RabbitMQSink : IQueueSink {

                private const string DefaultQueueSectionName = "RabbitMq";

                // Designed to be singleton
                private static IConnection Connection { get; set; }
                private static object Lock { get; } = new object();

                public Task PublishAsync(string queue, string msg, string exchange = null) =>
                        Task.Run(() => {
                                using (var channel = Connection.CreateModel()) {
                                        channel.QueueDeclarePassive(queue);
                                        var body = Encoding.UTF8.GetBytes(msg);
                                        channel.BasicPublish(exchange ?? string.Empty, queue, null, body);
                                }
                        });
                public IQueueSink UsingConfiguration(IConfiguration cfg, string name = null) {
                        lock(Lock) {
                                if (Connection == null) {
                                        var connectionFactory = new ConnectionFactory();
                                        cfg.GetSection(name ?? DefaultQueueSectionName).Bind(connectionFactory);
                                        Connection = connectionFactory.CreateConnection();
                                }
                        }
                        return this;
                }
        }
}
