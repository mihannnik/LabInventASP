
using LabInventASP.Infrastructure;
using LabInventASP.Interfaces;
using LabInventASP.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace LabInventASP.Services
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQBackgroundService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = configuration["RabbitMQ:User"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: configuration["RabbitMQ:Queue"], durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, EventArgs) =>
            {
                var content = Encoding.UTF8.GetString(EventArgs.Body.ToArray());

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var Repository = scope.ServiceProvider.GetRequiredService<IRepository<DeviceStatus>>();
                    foreach (DeviceStatus device in JsonSerializer.Deserialize<List<DeviceStatus>>(content))
                    {
                        Repository.Save(device);
                    }
                }

                _channel.BasicAck(EventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume("Devices", false, consumer);

            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
