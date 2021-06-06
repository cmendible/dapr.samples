using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace twitter.sentiment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SentimentController : ControllerBase
    {
        IConfiguration _configuration;
        private readonly ILogger<SentimentController> _logger;

        public SentimentController(IConfiguration configuration, ILogger<SentimentController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<double?> Post([FromBody] string content)
        {
            var credentials = new ApiKeyServiceClientCredentials(_configuration["cognitiveServicesKey"]);
            var textAnalyticsClient = new TextAnalyticsClient(credentials)
            {
                Endpoint = "https://westeurope.api.cognitive.microsoft.com/"
            };

            var result = await textAnalyticsClient.SentimentAsync(content);
            _logger.LogInformation($"Sentiment Score: {result.Score:0.00}");

            return result.Score;
        }
    }
}
