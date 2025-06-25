using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Models
{
    public class ChatSession
    {
        public string Id { get; set; } = string.Empty;
        public List<ChatMessage> Messages { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public string? Title { get; set; }
    }
}
