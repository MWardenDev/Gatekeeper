using Serilog;
using Gatekeeper.Models;

namespace Logging.Core {
    public class SerilogLogger :ILoggerService {
        private readonly ILogger _logger;

        public SerilogLogger() {
            _logger = Log.Logger;
        }

        public void LogReceived(MessageDto message) {
            _logger.Information("Received message from {SenderId} of type {MessageType} with payload: {Payload}",
                message.SenderId, message.MessageType, message.Payload);
        }

        public void LogRejected(MessageDto message, string reason) {
            _logger.Warning("Rejected message from {SenderId}. Reason: {Reason}", message.SenderId, reason);
        }

        public void LogRouted(MessageDto message, string destination) {
            _logger.Information("Message from {SenderId} routed to {Destination}", message.SenderId, destination);
        }
    }
}
