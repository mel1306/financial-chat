using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialChat.Models
{
    public class Message
    {
        public Message()
        {
            Date = DateTime.Now;
        }
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string UserID { get; set; }
        public virtual User Sender { get; set; }
    }
}
