using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Text;
using System.Threading;

namespace aspnetcore.dapr.secret
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            var rnd = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var reading = new
                {
                    Data = new { Temperature = rnd.Next(18, 42) }
                };

                await client.PostAsync(
                    "http://localhost:3501/v1.0/bindings/readings-output",
                    new StringContent(JsonSerializer.Serialize(reading, options), Encoding.UTF8, "application/json")
                );
            }

        }
    }
}
