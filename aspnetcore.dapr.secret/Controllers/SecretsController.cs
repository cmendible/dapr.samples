namespace aspnetcore.dapr.secret.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Dapr.Client;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class SecretsController : ControllerBase
    {
        private readonly ILogger<SecretsController> _logger;
        private readonly HttpClient _httpClient;

        public SecretsController(ILogger<SecretsController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get([FromServices] DaprClient client)
        {
            var result = await client.GetSecretAsync("demosecrets", "redisPass");
            return result["redisPass"];
        }

        [HttpGet("GetK8sSecret")]
        public async Task<ActionResult<string>> GetK8sSecret()
        {
            var result = await _httpClient.GetAsync("http://localhost:3500/v1.0/secrets/kubernetes/redis?metadata.namespace=dapr-test");
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}