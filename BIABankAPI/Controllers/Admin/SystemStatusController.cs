using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize]
    public class SystemStatusController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SystemStatusController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemStatus()
        {
            var newRelicApiKey = Environment.GetEnvironmentVariable("NEW_RELIC_API_KEY");
            var process = Process.GetCurrentProcess();

            var (applications, error) = await GetNewRelicAppsAsync(newRelicApiKey);

            return Ok(new
            {
                Api = new
                {
                    Uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime(),
                    MemoryUsageMB = Math.Round(process.WorkingSet64 / 1024.0 / 1024.0, 2)
                },
                Applications = applications,
                NewRelicConfigured = !string.IsNullOrEmpty(newRelicApiKey),
                NewRelicError = error,
                Timestamp = DateTime.UtcNow
            });
        }

        private async Task<(List<object>, string?)> GetNewRelicAppsAsync(string? apiKey)
        {
            var result = new List<object>();

            if (string.IsNullOrEmpty(apiKey))
                return (result, null);

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Api-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var query = new
                {
                    query = @"
                    {
                        actor {
                            entitySearch(query: ""domain = 'APM' AND name LIKE '%BIABank%'"") {
                                results {
                                    entities {
                                        ... on ApmApplicationEntityOutline {
                                            name
                                            reporting
                                            alertSeverity
                                        }
                                    }
                                }
                            }
                        }
                    }"
                };

                var content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.newrelic.com/graphql", content);

                if (!response.IsSuccessStatusCode)
                    return (result, $"API returned {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.GetArrayLength() > 0)
                    return (result, errors[0].GetProperty("message").GetString());

                var entities = doc.RootElement
                    .GetProperty("data")
                    .GetProperty("actor")
                    .GetProperty("entitySearch")
                    .GetProperty("results")
                    .GetProperty("entities");

                foreach (var entity in entities.EnumerateArray())
                {
                    result.Add(new
                    {
                        AppName = entity.GetProperty("name").GetString(),
                        Reporting = entity.GetProperty("reporting").GetBoolean(),
                        HealthStatus = MapAlertSeverity(entity.GetProperty("alertSeverity").GetString())
                    });
                }

                return (result, null);
            }
            catch (Exception ex)
            {
                return (result, ex.Message);
            }
        }

        private static string MapAlertSeverity(string? severity) => severity?.ToUpper() switch
        {
            "NOT_ALERTING" => "green",
            "WARNING" => "yellow",
            "CRITICAL" => "red",
            _ => "gray"
        };
    }
}
