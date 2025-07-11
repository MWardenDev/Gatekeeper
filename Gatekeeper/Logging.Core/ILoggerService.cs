using Gatekeeper.Models;

namespace Logging.Core
{
    public interface ILoggerService {
        void LogReceived(MessageDto message);
        void LogRejected(MessageDto message, string reason);
        void LogRouted(MessageDto message, string destination);

        // New general-purpose logging methods
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

}
