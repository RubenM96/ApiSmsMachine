using Serilog;
using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public class AreaSxSmsSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AreaSxSmsSender> _logger;
        private readonly string _password;

        private AreaSxSmsSender() { }
        public AreaSxSmsSender(HttpClient httpClient, ILogger<AreaSxSmsSender> logger, AreaSxOptions options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _password = options.Password;
        }

        /// <summary>
        /// Invia un form ad AreaSx con il testo, il numero e richiedendo opzionalmente la notifica di consegna.
        /// </summary>
        /// <param name="recipient">
        /// Il numero di telefono del destinatario a cui inviare l’SMS.
        /// Deve essere un numero valido nel formato previsto.
        /// </param>
        /// <param name="text">
        /// Il contenuto testuale del messaggio SMS da inviare.
        /// </param>
        /// <param name="notify">
        /// Indica se richiedere una notifica di consegna per l’SMS inviato.
        /// </param>
        /// <returns>
        /// Un <see cref="AreaSxSendResult"/> contenente l’esito dell’invio dell’SMS,
        /// lo stato e gli eventuali dettagli della risposta.
        /// </returns>
        /// <exception cref="Exception">
        /// Generata quando l’SMS non può essere inviato a causa di una risposta HTTP fallita
        /// o di un altro errore durante l’operazione.
        /// </exception>


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

            if (notify)
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
            Log.ForContext("AresSx", "AreaSxSenderResponse").Information(json);

            var result = System.Text.Json.JsonSerializer.Deserialize<AreaSxSendResult>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("SMS sent successfully to {Recipient}", recipient);

            return result;
        }
    }

}
