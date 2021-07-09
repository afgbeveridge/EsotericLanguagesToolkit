using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Eso.API.Shared {
        public class RabbitMQSink : IQueueSink {

                private const string DefaultQueueSectionName = "RabbitMq";

                // Designed to be singleton
                private static IConnection Connection { get; set; }
                private static object Lock { get; } = new object();

                public async Task PublishToExchangeAsync(string exchange, string msg) =>
                        await PublishAsync(msg, channel => channel.ExchangeDeclarePassive(exchange), exchange: exchange);


                public async Task PublishToQueueAsync(string queue, string msg) =>
                        await PublishAsync(msg, channel => channel.QueueDeclarePassive(queue), queue: queue);

                private async Task PublishAsync(string msg, Action<IModel> configure, string exchange = null, string queue = null) {
                        await Task.Run(() => {
                                using (var channel = Connection.CreateModel()) {
                                        configure(channel);
                                        var body = Encoding.UTF8.GetBytes(msg);
                                        channel.BasicPublish(exchange ?? string.Empty, queue ?? string.Empty, null, body);
                                }
                        });
                }

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
