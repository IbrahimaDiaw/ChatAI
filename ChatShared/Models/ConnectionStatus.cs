using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Models
{
    public class ConnectionStatus
    {
        public bool IsConnected { get; set; }
        public string Status { get; set; } = "Disconnected";
        public string? ConnectionId { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
