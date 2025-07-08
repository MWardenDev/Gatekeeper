using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gatekeeper.Models {
    public class RouteResult {
       public bool Success { get; set; }
        public string? Node { get; set; }
        public string? Error { get; set; }

        public string? TargetEndpoint { get; set; }
        public string? RouteType { get; set; }
    }
}
