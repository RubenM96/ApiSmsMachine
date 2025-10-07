using Serilog;
using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public class AreaSxSmsSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AreaSxSmsSender> _logger;
        private readonly string _password;

        public AreaSxSmsSender() {}
        public AreaSxSmsSender(HttpClient httpClient, ILogger<AreaSxSmsSender> logger, AreaSxOptions options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _password = options.Password;
        }

        public AreaSxSendResult SendSms(string recipient, string text, bool notify)
        {
            _logger.LogInformation("Sending SMS to {Recipient} with text: {Text} and notify: {Notify}", recipient, text, notify);
            
            using var request = new HttpRequestMessage(HttpMethod.Post, AreaSxSmsMachine.Endpoints.SendSms);

            var data = new Dictionary<string, string>
            {
                { "Pwd", _password },
                { "num", recipient },
                { "text", text }
            };

            if(notify)
                data.Add("notify", "1");

            request.Content = new FormUrlEncodedContent(data);

           // Console.WriteLine($"URL: {request.RequestUri}. Base URL: {_httpClient.BaseAddress}");
            using var response = _httpClient.Send(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send SMS to {Recipient}. Status Code: {StatusCode}", recipient, response.StatusCode);
                throw new Exception($"Failed to send SMS: {response.StatusCode}");
            }   
            
            var json = response.Content.ReadAsStringAsync().Result;

            //Serilog
            Log.Information(json);

            var result = System.Text.Json.JsonSerializer.Deserialize<AreaSxSendResult>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
           // Console.WriteLine($"AreaSxSendResult: {result.Errno} , {result.Errdesc}, {result.Index}");

            if (!result.IsSuccess)
                if(result.Refused)
                    throw new Exception("SMS Refused by AreaSx:" + result.Errdesc);
                else
                    throw new Exception(result.Errno);
                        
            _logger.LogInformation("SMS sent successfully to {Recipient}", recipient);           

            return result;
        }
    }

}
