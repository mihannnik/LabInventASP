using RabbitMQ.Client;
using FileParser.Interfaces;
using System.Text.Json;
using System.Text;

namespace FileParser.Services
{
    public class RabbitMQService(IConfiguration _configuration) : IRabbitMQService
    {
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { 
                HostName = _configuration["RabbitMQ:Hostname"],
                UserName = _configuration["RabbitMQ:User"],
                Password = _configuration["RabbitMQ:Password"]
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _configuration["RabbitMQ:Queue"],
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: _configuration["RabbitMQ:Queue"],
                               basicProperties: null,
                               body: body);
            }
        }
    }
}
