using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Routing.Core;
using Routing.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing.Core {
    public class RoutingStrategyService : IRoutingStrategyService {
        private readonly ILogger<RoutingStrategyService> _logger;
        private readonly IRouteRepository _routeRepository;

        public RoutingStrategyService(ILogger<RoutingStrategyService> logger, IRouteRepository routeRepository) {
            _logger = logger;
            _routeRepository = routeRepository;
        }

        public string ResolveTargetEndpoint(HttpContext context) {            
            var path = context.Request.Path.Value ?? string.Empty;
            _logger.LogDebug("Trying to match {context}", path);
            var endpoint = path.Trim('/').Split('/').FirstOrDefault();

            _logger.LogDebug("Resolved endpoint from path '{Path}' as '{Endpoint}'", path, endpoint);

            return endpoint ?? "UnknownEndpoint";
        }

        public async Task<RouteDefinition> DetermineRouteAsync(HttpContext context) {
            var endpoint = ResolveTargetEndpoint(context);

            _logger.LogDebug("The found endpoint is: {0}", endpoint);

            var route = await _routeRepository.GetRouteForEndpointAsync(endpoint);

            if(route == null) {
                _logger.LogWarning("No route found for endpoint: {Endpoint}", endpoint);
                throw new InvalidOperationException($"No route found for endpoint {endpoint}");
            }

            return route;
        }

        public async Task<RouteDefinition?> DetermineRouteAsync(GatekeeperMessage message) {
            var headers = message.Headers ?? new Dictionary<string, string>();
            var path = headers.TryGetValue("Path", out var p1) ? p1
                     : headers.TryGetValue(":path", out var p2) ? p2
                     : null;

            _logger.LogDebug("Extracted path from recovery message: {Path}", path);

            if(string.IsNullOrWhiteSpace(path)) {
                _logger.LogWarning("No path found in message headers.");
                return null;
            }

            var endpoint = path.Trim('/').Split('/').FirstOrDefault();
            _logger.LogDebug("Resolved endpoint from recovery path '{Path}' as '{Endpoint}'", path, endpoint);

            return await _routeRepository.GetRouteForEndpointAsync(endpoint);
        }
    }
}
