using Gatekeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recovery.Core.Models {
    public class MessageWrapper {
        public MessageDto Message { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public DateTimeOffset Timestamp { get; set; }
    }
}
