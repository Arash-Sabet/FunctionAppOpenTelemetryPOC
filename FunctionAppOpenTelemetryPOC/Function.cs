using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionAppOpenTelemetryPOC;

public class Function(ILogger<Function> logger)
{
    [Function("Function")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {        
        var response = req.CreateResponse();
        await response.WriteAsJsonAsync(new int[] { 12, 35, 3455, 123 });
        logger.LogInformation("A JSON object representing an array created and reported.");
        logger.FoodPriceChanged("Olive Oil", 34.09d);
        logger.LogWarning("Some warnings...");
        logger.LogError("Some erros...");
        logger.LogCritical("Some critical stuff...");
        return response;
    }
}
