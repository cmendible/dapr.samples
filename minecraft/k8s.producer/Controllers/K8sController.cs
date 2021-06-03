using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace k8s.producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class K8sController : ControllerBase
    {
        public class KubernetesEventWrapper
        {
            [JsonPropertyName("event")]
            public string Event { get; set; }

            [JsonPropertyName("oldVal")]
            public KubernetesEvent OldVal { get; set; }

            [JsonPropertyName("newVal")]
            public KubernetesEvent NewVal { get; set; }
        }

        public class KubernetesEvent
        {
            [JsonPropertyName("reason")]
            public string Reason { get; set; }

            [JsonPropertyName("message")]
            public object Message { get; set; }
        }

        private readonly ILogger<K8sController> _logger;

        public K8sController(ILogger<K8sController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/kubernetes-reader")]
        public async Task<ActionResult> Post([FromBody] KubernetesEventWrapper k8sEvent, [FromServices] DaprClient client)
        {
            if (k8sEvent.Event != "update")
            {
                var value = k8sEvent.OldVal.Message != null && k8sEvent.OldVal.Reason != null ? k8sEvent.OldVal : k8sEvent.NewVal;

                var message = $"K8s event received: {value.Reason} - {value.Message}";

                // Log k8sEvent
                _logger.LogInformation(message);

                // Publish k8sEvent
                await client.PublishEventAsync("messagebus", "k8sevents", message);
            }
            // Acknowledge message
            return Ok();
        }
    }
}
