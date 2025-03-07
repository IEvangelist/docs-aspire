---
ms.topic: include
---

[!INCLUDE [client-integration-health-checks](../../includes/client-integration-health-checks.md)]

The .NET Aspire Stack Exchange Redis output caching integration handles the following:

- Adds the health check when <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the container instance.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Stack Exchange Redis output caching integration uses the following Log categories:

- `Aspire.StackExchange.Redis`
- `Microsoft.AspNetCore.OutputCaching.StackExchangeRedis`

#### Tracing

The .NET Aspire Stack Exchange Redis output caching integration will emit the following Tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.StackExchangeRedis`

#### Metrics

The .NET Aspire Stack Exchange Redis output caching integration currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.
