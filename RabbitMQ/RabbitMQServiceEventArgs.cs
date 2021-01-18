using System;

namespace FinancialChat.RabbitMQ
{
    public class RabbitMQServiceEventArgs : EventArgs
    {
        public string  Text { get; set; }
        public DateTime Date { get; set; }
    }
}