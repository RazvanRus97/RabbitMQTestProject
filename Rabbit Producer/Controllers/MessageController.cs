using Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Rabbit_Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IConnectionFactory _queueFactory;

        public MessageController()
        {
            _queueFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://localhost:5672")
            };
        }

        [HttpPost]
        public IActionResult QueueMessage(MessageDTO message)
        {

            using var connection = _queueFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("message-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish(
                exchange: "",
                routingKey: "message-queue",
                basicProperties: null,
                body: body
                );
            Console.WriteLine("Message Queued at: {0}", DateTime.Now);
            return Ok();
        }
    }
}
