using Gatekeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing.Core {
    public class RoutingService :IRoutingService {
        private readonly ILogger<RoutingService> _logger;

        public RoutingService(ILogger<RoutingService> logger) {
            _logger = logger;
        }

        public async Task<RouteResult> RouteAsync(MessageDto message) {
            switch(message.MessageType.ToLowerInvariant()) {
                case "ping":
                    return new RouteResult {
                        Node = "Node-ping",
                        Success = true
                    };

                case "audit":
                    // Placeholder for deeper logic based on message.Payload
                    return await RouteAuditAsync(message);

                default: {
                    _logger.LogWarning("Unknown message type: {MessageType}", message.MessageType);
                    return new RouteResult {
                        Node = "Unknown",
                        Success = false,
                        Error = $"Unkonown message type: {message.MessageType}"
                    };
                }
            }
        }

        private Task<RouteResult> RouteAuditAsync(MessageDto message) {
            // In the future: parse Json payload, check clientId, etc.
            return Task.FromResult(new RouteResult {
                Node = "Node-audit",
                Success = true
            });
        }
    }
}
