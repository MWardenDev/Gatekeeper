using Auth.Core;
using Gatekeeper.Models;
using Logging.Core;
using Microsoft.AspNetCore.Mvc;
using Routing.Core;

namespace Gatekeeper.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IAuthenticator _authenticator;
        private readonly ILoggerService _logger;
        private readonly IRoutingStrategyService _routingStrategy;
        private readonly IRouteExecutorService _routeExecutor;

        public MessageController(IAuthenticator authencator, ILoggerService logger, 
                                 IRoutingStrategyService routingStrategy, IRouteExecutorService routeExecutor)
        {
            _authenticator = authencator;
            _logger = logger;
            _routingStrategy = routingStrategy;
            _routeExecutor = routeExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] MessageDto message) {
            if(!_authenticator.Authenticate(message)) {
                _logger.LogRejected(message, "Authentication failed");
                return StatusCode(StatusCodes.Status401Unauthorized, "Authentication failed");
            }

            if(!_authenticator.Authorize(message)) {
                _logger.LogRejected(message, "Authorization failed");
                return StatusCode(StatusCodes.Status403Forbidden, "Authorization failed");
            }

            _logger.LogReceived(message);

            var route = await _routingStrategy.DetermineRouteAsync(HttpContext);
            await _routeExecutor.ExecuteRouteAsync(route, message, HttpContext);

            return Accepted();
        }

    }
}
