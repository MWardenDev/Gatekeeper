using Gatekeeper.Models;
using Logging.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Routing.Core;
using System;
using System.Threading.Tasks;

namespace Recovery.Core {
    public class MessageRetryService :IMessageRetryService {
        private readonly IMessagePersistenceService _persistence;
        private readonly IRoutingStrategyService _routingStrategy;
        private readonly IRouteExecutorService _routeExecutor;
        private readonly ILoggerService _logger;

        public MessageRetryService(
            IMessagePersistenceService persistence,
            IRoutingStrategyService routingStrategy,
            IRouteExecutorService routeExecutor,
            ILoggerService logger) {
            _persistence = persistence;
            _routingStrategy = routingStrategy;
            _routeExecutor = routeExecutor;
            _logger = logger;
        }

        public async Task RetryMessageAsync(string fileName) {
            var wrapper = await _persistence.LoadMessageAsync(fileName);
            if(wrapper == null) {
                _logger.LogInfo($"Retry failed: could not load message from file {fileName}");
                return;
            }

            var context = new DefaultHttpContext();
            foreach(var kvp in wrapper.Headers)
                context.Request.Headers[kvp.Key] = kvp.Value;

            try {
                var route = await _routingStrategy.DetermineRouteAsync(context);
                await _routeExecutor.ExecuteRouteAsync(route, wrapper.Message, context);
                await _persistence.DeleteMessageAsync(fileName);
                _logger.LogInfo($"Retry successful for file: {fileName}");

            } catch(Exception ex) {
                _logger.LogError($"Retry failed for file {fileName}: {ex.Message}");
            }
        }

        public async Task RetryAllPendingMessagesAsync() {
            var messages = await _persistence.GetPendingMessagesAsync();

            foreach(var (fileName, _) in messages) {
                await RetryMessageAsync(fileName);
            }
        }
    }
}
