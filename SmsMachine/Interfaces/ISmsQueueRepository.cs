using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsQueueRepository
    {
        SmsQueue AddSmsQueue(SmsQueue smsQueue);
        SmsQueue? GetById(int id);
        List<SmsQueue> GetSmsQueueByCampaign(int CampaignId);
        bool Delete(int id);




    }
}
