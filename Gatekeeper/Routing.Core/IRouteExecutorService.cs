using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Routing.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing.Core {
    public interface IRouteExecutorService {
        Task ExecuteRouteAsync(RouteDefinition route, MessageDto message, HttpContext context);
    }
}
