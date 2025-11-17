using SmsMachine.Models;
using System.ComponentModel.DataAnnotations;

namespace SmsMachine.Services
{
    public interface ISmsQueueService
    {
        void EnqueueSms(Recipient recipient, string text, bool multipart, bool notify, int? campaignId);
        Task ProcessSmsQueueAsync(TimeSpan delay);

    }
}
