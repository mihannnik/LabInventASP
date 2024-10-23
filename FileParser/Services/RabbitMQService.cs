using RabbitMQ.Client;
using FileParser.Interfaces;
using System.Text.Json;
using System.Text;

namespace FileParser.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { 
                HostName = "172.20.147.2",
                UserName = "rmuser",
                Password = "rmpassword"
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Devices",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: "Devices",
                               basicProperties: null,
                               body: body);
            }
        }
    }
}
