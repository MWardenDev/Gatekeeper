namespace Gatekeeper.Models {
    public class MessageDto {
        public string MessageId { get; set; } = Guid.NewGuid().ToString(); // auto-generate if not provided
        public string SenderId { get; set; }
        public string MessageType { get; set; }
        public string Payload { get; set; }
    }
}
