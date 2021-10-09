namespace Dapr.Sensors.Interfaces
{
    using Dapr.Actors;
    using System;
    using System.Threading.Tasks;

    public interface ISensorActor : IActor
    {
        Task SetDataAsync(SensorData data);
    }

    public class SensorData
    {
        public string SensorId { get; set; }

        public DateTime Timestamp { get; set; }

        public double Temperature { get; set; }

        public double Energy { get; set; }
    }
}
