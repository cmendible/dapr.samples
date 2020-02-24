using System;
using System.Threading.Tasks;
using Temperature.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ignite.Temperature
{
    [Route("api/temperature")]
    public class TemperatureController : Controller
    {
        private readonly TemperatureHub _temperatureHub;

        public TemperatureController(TemperatureHub temperatureHub)
        {
            _temperatureHub = temperatureHub;
        }

        [HttpPost("report")]
        public async Task Report(double temperature)
        {
            var reading = new
            {
                Date = DateTime.Now,
                Temperature = temperature
            };

            await _temperatureHub.SendMessage(JsonConvert.SerializeObject(reading));
        }

        [HttpGet("generate")]
        public async Task Generate()
        {
            var rnd = new Random();

            for (var i = 0; i < 100; i++)
            {
                await Report(rnd.Next(18, 42));
                await Task.Delay(1000);
            }
        }
    }
}