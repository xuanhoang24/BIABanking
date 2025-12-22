using System.Text.Json;

namespace BankingSystemMVC.Infrastructure.Json
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
    }
}
