using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Models
{
    public class ChatMessage
    {
        public string Id { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsFromAI { get; set; }
    }
}
