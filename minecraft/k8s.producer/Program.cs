using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
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
        e.MapPost("/kubernetes-reader", KubernetesEvent);
    });
}).Build().Run();


async Task KubernetesEvent(HttpContext context)
{
    var daprClient = context.RequestServices.GetRequiredService<DaprClient>();
    var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();
    
    var k8sEvent = await JsonSerializer.DeserializeAsync<KubernetesEventWrapper>(context.Request.Body, serializerOptions);

     if (k8sEvent.Event != "update")
    {
        var value = k8sEvent.OldVal.Message != null && k8sEvent.OldVal.Reason != null ? k8sEvent.OldVal : k8sEvent.NewVal;

        var message = $"K8s event received: {value.Reason} - {value.Message}";

        Console.WriteLine(message);

        // Publish k8sEvent
        await daprClient.PublishEventAsync("messagebus", "k8sevents", message);
    }
}

public class KubernetesEventWrapper
{
    [JsonPropertyName("event")]
    public string Event { get; set; }

    [JsonPropertyName("oldVal")]
    public KubernetesEvent OldVal { get; set; }

    [JsonPropertyName("newVal")]
    public KubernetesEvent NewVal { get; set; }
}

public class KubernetesEvent
{
    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    [JsonPropertyName("message")]
    public object Message { get; set; }
}