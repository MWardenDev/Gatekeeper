using Serilog;
using Gatekeeper.Models;

namespace Logging.Core {
    public class SerilogLoggerService :ILoggerService {
        private readonly ILogger _logger;

        public SerilogLoggerService(ILogger logger) {
            _logger = logger;
        }

        public void LogReceived(MessageDto message) =>
            _logger.Information("Received message of type {MessageType} from {SenderId}", message.MessageType, message.SenderId);

        public void LogRejected(MessageDto message, string reason) =>
            _logger.Warning("Rejected message from {SenderId}. Reason: {Reason}", message.SenderId, reason);

        public void LogRouted(MessageDto message, string destination) =>
            _logger.Information("Message from {SenderId} routed to {Destination}", message.SenderId, destination);

        // New implementations
        public void LogInfo(string message) =>
            _logger.Information(message);

        public void LogWarning(string message) =>
            _logger.Warning(message);

        public void LogError(string message) =>
            _logger.Error(message);
    }

}

