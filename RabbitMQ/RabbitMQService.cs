using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using RabbitMQ.Client.Events;
using MessageQueue;

namespace FinancialChat.RabbitMQ
{
    public class RabbitMQService
    {
        private const string QueueName = "FinancialChatQueue";
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private EventHandler<RabbitMQServiceEventArgs> _messageConsumed;
        public event EventHandler<RabbitMQServiceEventArgs> MessageConsumed
        {
            add
            {
                if (_messageConsumed == null)
                {
                    _messageConsumed += value;
                }
            }
            remove
            {
                _messageConsumed -= value;
            }
        }

        public RabbitMQService(IOptions<RabbitMQInfo> rabbitMq)
        {
            var rabbitMqInfo = rabbitMq.Value;
            var connectionFactory = new ConnectionFactory
            {
                UserName = rabbitMqInfo.Username,
                Password = rabbitMqInfo.Password,
                VirtualHost = rabbitMqInfo.VirtualHost,
                HostName = rabbitMqInfo.HostName,
                Uri = new Uri(rabbitMqInfo.Uri)
            };
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (sender, e) => 
            {
                var body = e.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<string>(jsonMessage);
                ConsumeMessage(message);
            };
            _channel.BasicConsume(QueueName, true, _consumer);
        }

        protected virtual void OnMessageConsumed(RabbitMQServiceEventArgs e)
        {
            var handler = _messageConsumed;
            handler?.Invoke(this, e);
        }

        private void ConsumeMessage(string message)
        {
            var args = new RabbitMQServiceEventArgs {Text = message, Date = DateTime.Now};
            OnMessageConsumed(args);
        }
    }
}
