using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace ChatServiceApp.BackgroundServices
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private readonly IConnection _rabbitMQConnection;
        private readonly IModel _chatChannel;
        private readonly string _chatChannelName = "chat";

        public RabbitMQBackgroundService(IConnection rabbitMQConnection)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _chatChannel = _rabbitMQConnection.CreateModel();
            _chatChannel.QueueDeclare(queue: _chatChannelName, exclusive: false, autoDelete: false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(_chatChannel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received: {message}");
            };
            _chatChannel.BasicConsume(
                queue: _chatChannelName,
                autoAck: true,
                consumer: consumer);

            await Task.CompletedTask;
        }
    }
}
