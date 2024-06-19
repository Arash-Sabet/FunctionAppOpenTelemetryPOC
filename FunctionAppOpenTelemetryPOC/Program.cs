using Azure.Monitor.OpenTelemetry.Exporter;
using FunctionAppOpenTelemetryPOC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string aiConnString = "InstrumentationKey=MASKED;IngestionEndpoint=https://MASKED.in.applicationinsights.azure.com/;LiveEndpoint=https://westus.livediagnostics.monitor.azure.com/;ApplicationId=MASKED";
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        config
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
    })
    .ConfigureServices(services =>
    {
        services
        .AddOpenTelemetry()
        //.UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri("http://localhost:4317/"))
        .ConfigureResource(r => r.AddService("POC.Func.App"))
        .WithMetrics(metrics => metrics
            .AddInstrumentation(() => "POC.Instrumentation")
            .AddAzureMonitorMetricExporter(o => o.ConnectionString = aiConnString)
            .AddMeter("POC.Meter")
            .AddOtlpExporter(config =>
            {
                config.Endpoint = new Uri("http://localhost:4317/");
                config.Protocol = OtlpExportProtocol.Grpc;
            })
            .AddConsoleExporter())
        .WithTracing(c =>
        {
            c.AddInstrumentation(() => "POC.Instrumentation")
            .AddAzureMonitorTraceExporter(o => o.ConnectionString = aiConnString)
            .AddSource(nameof(Function))
            .AddSource("POC.Source")
            .AddOtlpExporter(config =>
            {
                config.Endpoint = new Uri("http://localhost:4317/");
                config.Protocol = OtlpExportProtocol.Grpc;
            })
            .AddConsoleExporter();
        });
    })
    .ConfigureLogging(logBuilder => logBuilder.AddOpenTelemetry(o =>
    {
        o.AddOtlpExporter("logging", c =>
        {
            c.Endpoint = new Uri("http://localhost:4317/");
            c.Protocol = OtlpExportProtocol.Grpc;
            c.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
        });
        o.AddAzureMonitorLogExporter(p => p.ConnectionString = aiConnString);
    }))
    .Build();
await host.RunAsync();

internal static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "Starting the app...")]
    public static partial void StartingApp(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Food `{name}` price changed to `{price}`.")]
    public static partial void FoodPriceChanged(this ILogger logger, string name, double price);
}