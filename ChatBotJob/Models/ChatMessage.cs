using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotJob.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string? UserId { get; set; }         
        public string Username { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsBot { get; set; }
        public string RoomId { get; set; } = "general"; 
    }
}