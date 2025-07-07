# Gatekeeper Logging Overview

## 1. Purpose of Gatekeeper

Gatekeeper serves as the **central entry point for all external API communication** into the court's ecosystem from external sources. It is designed to provide:

- **Authentication and Authorization** of each incoming message
- **Structured logging** of all accepted or rejected messages
- **Scalable routing** of validated messages to the appropriate delivery points
- **Threaded message processing** to support high-throughput operation
- **Modular architecture** to support future microservice extraction

Gatekeeper is hosted in **Azure App Services**, fronted by **Azure API Management (APIM)**. Each incoming API request is authenticated, logged, and routed as quickly as possible, with minimal latency and strong separation of concerns. This ensures both **security** and **observability** from the edge inward.

---

## 2. Configured Logging Fields

The following fields are automatically included in each log event via Serilog enrichers and properties. These fields are used for **traceability**, **diagnostics**, and **OpenTelemetry compatibility**.

| Field Name                | Description |
|--------------------------|-------------|
| `Timestamp`              | UTC timestamp of the log entry (automatic) |
| `Level`                  | Log level (e.g., Information, Warning, Error) |
| `Message`                | Main log message string |
| `ThreadId`               | ID of the thread executing the request (helps trace async flow) |
| `MachineName`            | Name of the machine or container running the service |
| `ProcessId`              | Numeric process ID for the app |
| `ProcessName`            | Name of the application process |
| `AspNetCoreEnvironment`  | Current environment (Development, Staging, Production, etc.) |
| `service.name`           | Logical name of the service (`Gatekeeper`) |
| `service.version`        | Current deployed version (`1.0.0`, etc.) |
| `service.instance.id`    | Unique identifier for the running instance (typically machine name or container ID) |
| `trace_id`               | OpenTelemetry-compatible trace ID (placeholder for now) |
| `span_id`                | OpenTelemetry-compatible span ID (placeholder for now) |

> 🔧 The `trace_id` and `span_id` fields are currently static. These will be dynamically generated or extracted from request headers in a future step to support full distributed tracing.

Logs are currently written to:
- **Console** output for real-time visibility
- **File** storage under `Logs/gatekeeper-log.txt`, with daily rolling enabled

---

## Coming Soon

Planned future enhancements include:

- Dynamic `trace_id` and `span_id` generation per request
- Correlation ID header propagation
- Optional database or cloud-based log sinks (e.g., Azure Monitor or Seq)
- Logging standard procedure document for development teams

