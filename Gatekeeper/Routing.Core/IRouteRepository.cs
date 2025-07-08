// File: Interfaces/IRouteRepository.cs
using Routing.Core.Models;

namespace Routing.Core {
    public interface IRouteRepository {
        Task<RouteDefinition?> GetRouteForEndpointAsync(string endpoint);
    }
}
