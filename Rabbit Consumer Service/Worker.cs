using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit_Consumer_Service.Models;
using Rabbit_Consumer_Service.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Contracts;

namespace Rabbit_Consumer_Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ConnectionFactory queueFactory;
        private readonly IMessageRepository _messageRepository;
        public Worker(ILogger<Worker> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            queueFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://localhost:5672")
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var connection = queueFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("message-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var content = Encoding.UTF8.GetString(body);
                var message = ConvertToMessage(content);
                _messageRepository.Add(message);
                Thread.Sleep(5000);
                Console.WriteLine();
            };
            channel.BasicConsume("message-queue", true, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private Message ConvertToMessage(string content)
        {
            var dto = JsonConvert.DeserializeObject<MessageDTO>(content);
            var message = new Message()
            {
                Body = dto.Content,
                ReceivedDate = DateTime.Now
            };
            return message;
        }
    }
}
