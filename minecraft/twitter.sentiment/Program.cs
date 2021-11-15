using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring;
using Dapr.Extensions.Configuration;
using System;

var client = new DaprClientBuilder()
    .Build();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddDaprSecretStore("secrets", client)
    .Build();

WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    s.AddSingleton<DaprClient>(client);
    s.AddSingleton(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    });
})
.UseConfiguration(config)
.Configure(app =>
{
    app.UseRouting();
    app.UseCloudEvents();
    app.UseEndpoints(e =>
    {
        e.MapSubscribeHandler();
        e.MapPost("/sentiment", Sentiment);
    });
    
}).Build().Run();

async Task Sentiment(HttpContext context)
{
    var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
    var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();

    var credentials = new ApiKeyServiceClientCredentials(configuration["cognitiveServicesKey"]);
    var textAnalyticsClient = new TextAnalyticsClient(credentials)
    {
        Endpoint = "https://westeurope.api.cognitive.microsoft.com/"
    };

    var content = await new StreamReader(context.Request.Body).ReadToEndAsync();

    var result = await textAnalyticsClient.SentimentAsync(content);
    Console.WriteLine($"Sentiment Score: {result.Score:0.00}");

    context.Response.ContentType = "application/json";
    await JsonSerializer.SerializeAsync(context.Response.Body, result.Score, serializerOptions);
}