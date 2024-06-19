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
        .ConfigureResource(r => r.AddService("POC.Func.App"))
        .WithLogging(logging => logging.AddConsoleExporter());
    })
    .Build();
await host.RunAsync();

internal static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "Starting the app...")]
    public static partial void StartingApp(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Food `{name}` price changed to `{price}`.")]
    public static partial void FoodPriceChanged(this ILogger logger, string name, double price);
}
