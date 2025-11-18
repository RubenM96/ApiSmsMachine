using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsQueueService : ISmsQueueService
    {
        private readonly ILogger<SmsQueueService> _logger;
        private readonly ISmsQueueRepository _smsQueueRepository;
        private readonly ISmsService _smsService;

        public SmsQueueService(ILogger<SmsQueueService> logger, ISmsQueueRepository smsQueueRepository, ISmsService smsService)
        {
            _logger = logger;
            _smsQueueRepository = smsQueueRepository;
            _smsService = smsService;
        }

        public void EnqueueSms(Recipient recipient, string text, bool multipart, bool notify, int? campaignId)
        {
            var smsQueue = new SmsQueue(recipient, text, multipart, notify, campaignId);
            _smsQueueRepository.AddSmsQueue(smsQueue);
            _logger.LogInformation("Enqueued SMS for {Recipient} (campaign {CampaignId})", recipient.Value, campaignId);
        }

        public async Task ProcessSmsQueueAsync(TimeSpan delay, CampaignSms campaign)
        {
            await Task.Delay(delay);


        }


    }
}
