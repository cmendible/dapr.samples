using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Sensors.Actors;

WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    s.AddDaprClient();
    s.AddActors(options =>
        {
            // Register actor types and configure actor settings
            options.Actors.RegisterActor<SensorActor>();
        });
}).
Configure(app =>
{
    app.UseRouting();
    app.UseEndpoints(e =>
    {
        // Register actors handlers that interface with the Dapr runtime.
        e.MapActorsHandlers();
    });
}).Build().Run();