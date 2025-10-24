using SmsMachine.Models;

namespace SmsMachine.Services
{
    public class SmsQueueService : ISmsQueueService
    {
        private readonly ILogger<SmsQueueService> _logger;
        //private readonly ISmsQueueRepositort _smsQueueRepository;

        public SmsQueueService(ILogger<SmsQueueService> logger)
        {
            _logger = logger;
        }

        public void EnqueueSms(Recipient recipient, string text, bool multipart, bool notify, int? campaignId)
        {
            var smsQueue = new SmsQueue(recipient, text, multipart, notify, campaignId);
            //_smsQueueRepository.AddSmsQueue(smsQueue);
        }


    }
}
