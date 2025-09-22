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
            //formattare i dati come nel modello SmsInbound


        }
    }
}
