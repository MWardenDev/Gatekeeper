// File: InMemoryRouteRepository.cs
using Microsoft.Extensions.Options;
using Routing.Core.Config;
using Routing.Core.Models;

namespace Routing.Core {
    public class InMemoryRouteRepository :IRouteRepository {
        private readonly Dictionary<string, RouteDefinition> _routes;

        public InMemoryRouteRepository(IOptions<RouteDefinitionMap> routeOptions) {
            _routes = new Dictionary<string, RouteDefinition>(routeOptions.Value, StringComparer.OrdinalIgnoreCase);
        }

        public Task<RouteDefinition?> GetRouteForEndpointAsync(string endpoint) {
            _routes.TryGetValue(endpoint, out var route);
            return Task.FromResult(route);
        }

        public void AddRoute(string key, RouteDefinition route) {
            _routes[key] = route;
        }
    }
}
