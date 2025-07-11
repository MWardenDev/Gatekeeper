using Gatekeeper.Models;
using Logging.Core;
using System;

namespace Logging.Core {
    public class ConsoleLogger :ILoggerService {
        public void LogReceived(MessageDto message) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[RECEIVED] Message of type '{message.MessageType}' from '{message.SenderId}'");
            Console.ResetColor();
        }

        public void LogRejected(MessageDto message, string reason) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[REJECTED] Message from '{message.SenderId}' rejected. Reason: {reason}");
            Console.ResetColor();
        }

        public void LogRouted(MessageDto message, string destination) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[ROUTED] Message from '{message.SenderId}' routed to '{destination}'");
            Console.ResetColor();
        }

        public void LogInfo(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }

        public void LogWarning(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] {message}");
            Console.ResetColor();
        }

        public void LogError(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ResetColor();
        }
    }
}
