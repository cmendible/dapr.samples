using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Dapr.Sensors.Interfaces;

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
        e.MapGet("/average/{id}",
            async c =>
            {
                var daprClient = e.ServiceProvider.GetRequiredService<DaprClient>();
                var serializerOptions = e.ServiceProvider.GetRequiredService<JsonSerializerOptions>();

                var deviceId = c.Request.RouteValues["id"].ToString();
                var data = await daprClient.GetStateAsync<SensorData>("statedb", deviceId);

                c.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(c.Response.Body, data, serializerOptions);
            });
        e.MapPost("average",
            async c =>
            {
                var daprClient = c.RequestServices.GetRequiredService<DaprClient>();
                var serializerOptions = c.RequestServices.GetRequiredService<JsonSerializerOptions>();

                var data = await JsonSerializer.DeserializeAsync<SensorData>(c.Request.Body, serializerOptions);

                await daprClient.SaveStateAsync<SensorData>("statedb", data.SensorId, data);
            }).WithTopic("messagebus", "temperature");
    });
}).Build().Run();