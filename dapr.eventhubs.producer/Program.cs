using System.Net.Http;
using System.Threading.Tasks;

namespace aspnetcore.dapr.secret
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient();

            for (var i = 0; i < 5; i++)
            {
                await client.PostAsync(
                    "http://localhost:3500/v1.0/bindings/readings-output",
                    new StringContent("25")
                );
            }
        }
    }
}
