using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsQueueRepository
    {
        SmsQueue AddSmsQueue(SmsQueue smsQueue);
        SmsQueue? GetById(int id);
        bool Delete(int id);
    }
}
