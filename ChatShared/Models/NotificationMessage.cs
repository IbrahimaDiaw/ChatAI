using ChatShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Models
{
    public class NotificationMessage
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
