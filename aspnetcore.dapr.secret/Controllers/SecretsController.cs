using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore.dapr.secret.Controllers
{
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
        public async Task<ActionResult<string>> Get()
        {
            var result = await _httpClient.GetAsync("http://localhost:3500/v1.0/secrets/azurekeyvault/redis");
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("/GetK8sSecret")]
        public async Task<ActionResult<string>> GetK8sSecret()
        {
            var result = await _httpClient.GetAsync("http://localhost:3500/v1.0/secrets/kubernetes/redis");
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
