using Serilog;
using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public class AreaSxSmsDiscard : ISmsDiscard
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<AreaSxSmsDiscard> _logger;
        private readonly string _password;

        private AreaSxSmsDiscard() { }
        public AreaSxSmsDiscard(HttpClient httpClient, ILogger<AreaSxSmsDiscard> logger, AreaSxOptions options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _password = options.Password;
        }

        public AreaSxSmsNotSent CheckDiscardSms()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, AreaSxSmsMachine.Endpoints.DiscardSms);

            var data = new Dictionary<string, string>
            {
                { "Pwd", _password }
            };

            request.Content = new FormUrlEncodedContent(data);

            using var response = _httpClient.Send(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to check discarded SMS. Status Code: {StatusCode}", response.StatusCode);
                throw new Exception($"Failed to check discarded SMS: {response.StatusCode}");
            }

            var json = response.Content.ReadAsStringAsync().Result;

            Log.Information(json);

            var result = System.Text.Json.JsonSerializer.Deserialize<AreaSxSmsNotSent>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("Discarded SMS check completed. Result: {result.Errno} , {result.Errdesc}, {result.SmsTxErrIdx}", result.Errno, result.Errdesc, result.SmsTxErrIdx);

            return result;
        }

    }
}
