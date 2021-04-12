using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit_Consumer_Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ConnectionFactory queueFactory;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
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
                var message = Encoding.UTF8.GetString(body);
                Thread.Sleep(5000);
                Console.WriteLine(message);
            };
            channel.BasicConsume("message-queue", true, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
