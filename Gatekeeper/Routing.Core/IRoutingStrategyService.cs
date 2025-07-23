// File: IRoutingStrategyService.cs
using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Routing.Core.Models;

namespace Routing.Core {
    public interface IRoutingStrategyService {
        Task<RouteDefinition> DetermineRouteAsync(GatekeeperMessage message);

        /// <summary>
        /// Resolves the logical target endpoint from the incoming request (typically the path or headers).
        /// Used primarily for legacy client support.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>The resolved endpoint string.</returns>
        string ResolveTargetEndpoint(HttpContext context);
    }
}

