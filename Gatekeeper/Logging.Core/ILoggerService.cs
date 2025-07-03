using Gatekeeper.Models;

namespace Logging.Core
{
    public interface ILoggerService
    {
        void LogReceived(MessageDto message);
        void LogRejected(MessageDto message, string reason);
        void LogRouted(MessageDto message, string destination);
    }
}
