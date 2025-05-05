namespace NB_Project.Model
{
    public class ChatSummary
    {
        public int Id { get; set; }

        public string? User1Id { get; set; } // أول مستخدم
        public string? User2Id { get; set; } // ثاني مستخدم
        public string? User1Name { get; set; } // اسم أول مستخدم
        public string? User2Name { get; set; } // اسم ثاني مستخدم
        public string? LastMessage { get; set; } // آخر رسالة تمت
        public DateTime LastMessageTime { get; set; }

    }
}
