using Logging.Core;
using Gatekeeper.Models;

namespace Gatekeeper.Api.Services
{
    public class ConsoleLogger : ILoggerService
    {
        public void LogReceived(MessageDto message)
        {
            Console.WriteLine($"[Received] From: {message.SenderId}, Type: {message.MessageType}, Payload: {message.Payload}");
        }

        public void LogRejected(MessageDto message, string reason)
        {
            Console.WriteLine($"[Rejected] From: {message.SenderId}, Reason: {reason}");
        }

        public void LogRouted(MessageDto message, string destination)
        {
            Console.WriteLine($"[Routed] Message from {message.SenderId} sent to {destination}");
        }
    }
}
