using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace twitter.producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly ILogger<TweetController> _logger;

        public TweetController(ILogger<TweetController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/tweets")]
        public async Task<ActionResult> Post([FromBody] TwitterQueryResponse tweet, [FromServices] DaprClient client)
        {
            var content = tweet.FullText;
            if (content == "")
            {
                content = tweet.Text;
            }

            var message = $"{tweet.User.ScreenName} said: {content}";

            // Log tweet
            _logger.LogInformation(message);

            // Publish tweet
            await client.PublishEventAsync("messagebus", "tweets", message);

            // Acknowledge message
            return Ok();
        }
    }
}
