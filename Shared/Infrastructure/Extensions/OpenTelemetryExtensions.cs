using FastEndpoints.OpenTelemetry;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class OpenTelemetryExtensions
{
    public static OpenTelemetryBuilder ConnectBackendServices(this OpenTelemetryBuilder builder) =>
        builder
            .WithTracing(static tracing =>
            {
                tracing.AddFastEndpointsInstrumentation();
                tracing.AddRedisInstrumentation();
                tracing.AddNpgsql();
                tracing.AddSource(DiagnosticHeaders.DefaultListenerName);
                tracing.AddSource("Marten");
            })
            .WithMetrics(static metrics =>
            {
                metrics.AddNpgsqlInstrumentation();
                metrics.AddMeter("Marten");
                metrics.AddMeter(InstrumentationOptions.MeterName);
            });
}