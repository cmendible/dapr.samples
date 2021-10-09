
namespace Dapr.Sensors.Client
{
    using System;
    using System.Threading.Tasks;
    using Dapr.Actors;
    using Dapr.Actors.Client;
    using Dapr.Sensors.Interfaces;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Startup up...");

            while (true)
            {
                var random = new Random();

                var sensorData = new SensorData
                {
                    SensorId = "1",
                    Temperature = random.NextDouble() * (60 - 10) + 10,
                    Energy = random.NextDouble() * (10 - 1) + 1,
                    Timestamp = DateTime.UtcNow
                };

                // Registered Actor Type in Actor Service
                var actorType = "SensorActor";

                // An ActorId uniquely identifies an actor instance
                // If the actor matching this id does not exist, it will be created
                var actorId = new ActorId("1");

                // Create the local proxy by using the same interface that the service implements.
                //
                // You need to provide the type and id so the actor can be located. 
                var proxy = ActorProxy.Create<ISensorActor>(actorId, actorType);

                // Now you can use the actor interface to call the actor's methods.
                Console.WriteLine($"Calling SetDataAsync on {actorType}:{actorId}...");
                await proxy.SetDataAsync(sensorData);
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
