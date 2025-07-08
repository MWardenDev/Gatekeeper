// File: Services/InMemoryRouteRepository.cs
using Routing.Core.Models;

namespace Routing.Core {
    public class InMemoryRouteRepository :IRouteRepository {
        private readonly Dictionary<string, RouteDefinition> _routes = new()
        {
            { "GetCaseDetails", new RouteDefinition {
                TargetType = "Http",
                TargetAddress = "https://internal-api.local/cases/details"
            }},
            { "UploadDocument", new RouteDefinition {
                TargetType = "Ftp",
                TargetAddress = "ftp://internal.ftp.server/uploads"
            }}
        };

        public Task<RouteDefinition?> GetRouteForEndpointAsync(string endpoint) {
            _routes.TryGetValue(endpoint, out var route);
            return Task.FromResult(route);
        }
    }
}
