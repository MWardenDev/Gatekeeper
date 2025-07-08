using Gatekeeper.Models;
using Routing.Core;
using Routing.Core.Models;

namespace Gatekeeper.Api.Services {
    public class RouteExecutorService :IRouteExecutorService {

        private readonly ILogger<RouteExecutorService> _logger;

        public RouteExecutorService(ILogger<RouteExecutorService> logger) {
            _logger = logger;
        }

        public async Task ExecuteRouteAsync(RouteDefinition route, MessageDto message, HttpContext context) {
            switch(route.TargetType.ToLowerInvariant()) {
                case "service":
                    await SendToHttpService(message);
                    break;
                case "ftp":
                    await SendToFtp(message);
                    break;
                case "file":
                    await SendToFile(message);
                    break;
                case "email":
                    await SendEmail(message);
                    break;
                default:
                    _logger.LogWarning("Unknown route target: {Route}", route);
                    break;
            }
        }

        private Task SendToHttpService(MessageDto message) {
            // TODO: Add HttpClient logic
            _logger.LogInformation("Sending to HTTP service...");
            return Task.CompletedTask;
        }

        private Task SendToFtp(MessageDto message) {
            // TODO: Add FTP logic
            _logger.LogInformation("Sending to FTP...");
            return Task.CompletedTask;
        }

        private Task SendToFile(MessageDto message) {
            // TODO: Add file logic
            _logger.LogInformation("Sending to File");
            return Task.CompletedTask;
        }

        private Task SendEmail(MessageDto message) {
            // TODO: Add E-mail logic
            _logger.LogInformation("Sending E-mail");
            return Task.CompletedTask;
        }
    }
}
