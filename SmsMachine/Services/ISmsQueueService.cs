using SmsMachine.Models;

namespace SmsMachine.Services
{
    public interface ISmsQueueService
    {
        void EnqueueSms(Recipient recipient, string text, bool multipart, bool notify, int? campaignId);

        public List<SmsQueue> GetSmsQueueByCampaign(int campaignId);
        public bool DeleteSmsInQueue(int id);
    }
}
