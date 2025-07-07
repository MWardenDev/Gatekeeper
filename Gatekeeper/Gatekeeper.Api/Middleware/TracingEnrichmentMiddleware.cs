using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Diagnostics;

namespace Gatekeeper.Api.Middleware {
    public class TracingEnrichmentMiddleware {
        private readonly RequestDelegate _next;

        public TracingEnrichmentMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            // Generate trace_id and span_id if not supplied
            var traceId = ActivityTraceId.CreateRandom().ToHexString();
            var spanId = ActivitySpanId.CreateRandom().ToHexString();

            using(LogContext.PushProperty("trace_id", traceId))
            using(LogContext.PushProperty("span_id", spanId)) {
                await _next(context);
            }
        }
    }
}
 