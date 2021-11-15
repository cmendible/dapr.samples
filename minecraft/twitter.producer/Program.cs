using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using twitter.producer;
using System;

WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    s.AddDaprClient();
    s.AddSingleton(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    });
}).
Configure(app =>
{
    app.UseRouting();
    app.UseCloudEvents();
    app.UseEndpoints(e =>
    {
        e.MapSubscribeHandler();
        e.MapPost("/tweets", Tweets);
    });
}).Build().Run();


async Task Tweets(HttpContext context)
{
    var daprClient = context.RequestServices.GetRequiredService<DaprClient>();
    var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();
    
    var tweet = await JsonSerializer.DeserializeAsync<TwitterQueryResponse>(context.Request.Body, serializerOptions);

    await daprClient.SaveStateAsync("statedb", tweet.IdStr, tweet);

    var content = tweet.FullText;
    if (content == "")
    {
        content = tweet.Text;
    }

    var message = $"{tweet.User.ScreenName} said: {content}";

    // Log tweet
    Console.WriteLine(message);

    // Publish tweet
    await daprClient.PublishEventAsync("messagebus", "tweets", message);
}