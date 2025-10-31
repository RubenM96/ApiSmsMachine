using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsQueueService : ISmsQueueService
    {
        private readonly ILogger<SmsQueueService> _logger;
        private readonly ISmsQueueRepository _smsQueueRepository;

        public SmsQueueService(ILogger<SmsQueueService> logger, ISmsQueueRepository smsQueueRepository)
        {
            _logger = logger;
            _smsQueueRepository = smsQueueRepository;
        }

        public void EnqueueSms(Recipient recipient, string text, bool multipart, bool notify, int? campaignId)
        {
            var smsQueue = new SmsQueue(recipient, text, multipart, notify, campaignId);
            _smsQueueRepository.AddSmsQueue(smsQueue);
            _logger.LogInformation("Enqueued SMS for {Recipient} (campaign {CampaignId})", recipient.Value, campaignId);
        }

       
    }
}
