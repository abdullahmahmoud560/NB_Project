using System.ComponentModel.DataAnnotations.Schema;

namespace NB_Project.Model
{
    public class ChatMessage
    {
        [Column("MessageId")]
        public int Id { get; set; }
        public string? SenderId { get; set; }  // رقم المرسل
        public string? ReceiverId { get; set; } // رقم المستلم
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

}
