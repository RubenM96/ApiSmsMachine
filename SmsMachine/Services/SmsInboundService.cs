using SmsMachine.Infrastructure.Utils;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsInboundService : ISmsInbound 
    {
        private readonly ILogger<SmsInboundService> _logger;
        private readonly ISmsInboundRepository _smsInboundRepository;

        public SmsInboundService(ILogger<SmsInboundService> logger, ISmsInboundRepository smsInboundRepository)
        {
            _logger = logger;
            _smsInboundRepository = smsInboundRepository;
        }

        public void SmsInbound(string recipient, string text, string date)
        {
            // parse della data
            DateTime receivedAtUtc;
            try
            {
                receivedAtUtc = SmsDateParser.ParseDateFromSmsMachine(date);
            }
            catch (Exception ex) 
            {
                _logger.LogWarning(ex, "Formato data non valido: {Date}", date);
                receivedAtUtc = DateTime.UtcNow;
            }
            // controllo lunghezza messaggio 
            if (!string.IsNullOrEmpty(text) && text.Length > 300)
                text = text[..300];

            var smsInbound = new SmsInbound(new Recipient(recipient), text, multipart: false, receivedAt: receivedAtUtc);
            

            try 
            {
                _smsInboundRepository.AddSmsInbound(smsInbound);
                _logger.LogInformation("Inbound SMS saved: Id={Id}, Recipient={Recipient}, Text={Text}", smsInbound.Id, recipient, text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save SMS for Recipient = {Recipient}", recipient);
                throw;
            }
        }
    }
}
