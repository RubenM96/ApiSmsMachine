using SmsMachine.Interfaces;
using SmsMachine.Models;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

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
            //bool multipart = false;
            ////controlli input
            //if (text.Length > 160)
            //    multipart = true;

            //if (text.Length > 300)
            //    text = text.Substring(0, 300);


            //formattare i dati come nel modello SmsInbound
            date = date.Substring(0, 19).Trim().Replace("-", "/");
            DateTime dateTime = DateTime.ParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

            var smsInbound = new SmsInbound(new Recipient(recipient), text, false, dateTime);

        }
    }
}
