namespace Dapr.Sensors.Actors
{
    using Dapr.Actors.Runtime;
    using Dapr.Client;
    using Dapr.Sensors.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Actor(TypeName = "SensorActor")]
    internal class SensorActor : Actor, ISensorActor
    {
        private const string STATE_KEY = "device_data";
        private const string DEVICE_DATA_AGGREGATION_TIMER = "device_data_timer";

        private readonly DaprClient daprClient;

        public SensorActor(ActorHost host, DaprClient client)
            : base(host)
        {
            daprClient = client;
            Console.WriteLine($"Actor {this.Id} instantiated!");
        }

        public async Task SetDataAsync(SensorData data)
        {
            Console.WriteLine($"Received: {data}");

            var existingState = await this.StateManager.TryGetStateAsync<IList<SensorData>>(STATE_KEY);
            var newState = new List<SensorData>();

            if (existingState.HasValue)
            {
                newState = existingState.Value.ToList();
            }
            else
            {
                newState = new List<SensorData>();
            }

            newState.Add(data);
            await this.StateManager.SetStateAsync(STATE_KEY, newState);

            Console.WriteLine($"Stored Data for sensor {data.SensorId} in Actor {this.Id}");
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected async override Task OnActivateAsync()
        {
            // Provides opportunity to perform some optional setup.
            Console.WriteLine($"Activating actor id: {this.Id}");

            await this.RegisterTimerAsync(
                DEVICE_DATA_AGGREGATION_TIMER,
                nameof(HandleTimerCallbackAsync),
                null, TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10));

            Console.WriteLine($"Registered timer {DEVICE_DATA_AGGREGATION_TIMER}");

            await base.OnActivateAsync();
        }

        /// <summary>
        /// This method is called whenever an actor is deactivated after a period of inactivity.
        /// </summary>
        protected async override Task OnDeactivateAsync()
        {
            // Provides Opporunity to perform optional cleanup.
            Console.WriteLine($"Deactivating actor id: {this.Id}");

            await this.UnregisterTimerAsync(DEVICE_DATA_AGGREGATION_TIMER);
            Console.WriteLine($"Unregistered timer {DEVICE_DATA_AGGREGATION_TIMER}");

            await base.OnDeactivateAsync();
        }

        private async Task HandleTimerCallbackAsync(object data)
        {
            Console.WriteLine($"ReceiveReminderAsync for ActorId {this.Id} is called!");

            var existingState = await this.StateManager.TryGetStateAsync<IList<SensorData>>(STATE_KEY);

            if (existingState.HasValue && existingState.Value.Any())
            {
                var sensorData = existingState.Value.ToList();

                Console.WriteLine($"Processing {sensorData.Count} temperature measurements for Sensor {this.Id}");

                var averageDeviceData = new SensorData
                {
                    SensorId = sensorData.First().SensorId,
                    Timestamp = DateTime.UtcNow,
                    Temperature = sensorData.Select(data => data.Temperature).Average()
                };

                await daprClient.PublishEventAsync("messagebus", "temperature", averageDeviceData);

                Console.WriteLine($"Message sent from Actor {this.Id} with average temperature {averageDeviceData.Temperature}");

                await this.StateManager.SetStateAsync(STATE_KEY, new List<SensorData>());
            }
        }
    }
}