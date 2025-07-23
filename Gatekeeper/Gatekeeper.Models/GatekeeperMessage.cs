namespace Gatekeeper.Models {
    public class GatekeeperMessage {
        public MessageDto Message { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
