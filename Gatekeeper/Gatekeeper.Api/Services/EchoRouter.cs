using Routing.Core;
using Gatekeeper.Models;
using Logging.Core;

namespace Gatekeeper.Api.Services
{
    public class EchoRouter: IMessageRouter
    {
        private readonly ILoggerService _logger;

        public EchoRouter(ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task RouteAsync(MessageDto message)
        {
            // Simulate routing delay
            await Task.Delay(100);

            // Dummy "routing": echo back the message type as destination
            string destination = $"Node-{message.MessageType}";
            _logger.LogRouted(message, destination);
        }
    }
}
