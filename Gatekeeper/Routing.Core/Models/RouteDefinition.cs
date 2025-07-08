namespace Routing.Core.Models {
    public class RouteDefinition {
        public string TargetType { get; set; } = string.Empty;  // e.g., "Http", "Ftp", "File", "Email"
        public string TargetAddress { get; set; } = string.Empty;
        public Dictionary<string, string>? Metadata { get; set; }  // Optional additional config (e.g., port, credentials)
    }
}

