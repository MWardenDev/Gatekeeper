using Gatekeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing.Core {
    public interface IRoutingService {
        Task<RouteResult> RouteAsync(MessageDto message);
    }
}
