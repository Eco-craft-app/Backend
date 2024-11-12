using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HackHeroes.Recycle.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddOtlp(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("RecycleAPI"))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

                metrics.AddOtlpExporter();
            })
             .WithTracing(tracing =>
             {
                 tracing
                     .AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation()
                     .AddEntityFrameworkCoreInstrumentation();

                 tracing.AddOtlpExporter();
             });

    }

}
