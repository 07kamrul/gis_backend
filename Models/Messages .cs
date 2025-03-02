﻿namespace gis_backend.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;


        public User Sender { get; set; }

        public User Receiver { get; set; }
    }
}
