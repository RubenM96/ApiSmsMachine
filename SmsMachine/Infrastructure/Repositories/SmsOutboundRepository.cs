using SmsMachine.Api.Infrastructure.Utils;
using SmsMachine.Infrastructure.Data;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class SmsOutboundRepository : ISmsOutboundRepository
    {
        private readonly SmsDbContext _context;

        public SmsOutboundRepository(SmsDbContext context)
        {
            _context = context;
        }

        public SmsOutbound AddSms(SmsOutbound sms)
        {
            _context.Set<SmsOutbound>().Add(sms);
            _context.SaveChanges();
            return sms;
        }

        public void ClearErrors()
        {
            _context.ChangeTracker.AcceptAllChanges();
        }

        public SmsOutbound? GetSmsById(int id)
        {
            return _context.Set<SmsOutbound>().Find(id);
        }

        public void UpdateSms(SmsOutbound sms)
        {
            var existing = _context.Set<SmsOutbound>().Find(sms.Id);
            if (existing == null)
                throw new Exception("Sms not found");

            _context.Entry(existing).CurrentValues.SetValues(sms);
            _context.SaveChanges();
        }

        public List<SmsOutbound> GetAllSmsByCampaignId(int campaignId)
        {
            return _context.Set<SmsOutbound>().Where(s => s.CampaignId == campaignId).ToList();
        }

        public void DeleteAllSmsByCampaignId(int campaignId)
        {
            var smsList = _context.Set<SmsOutbound>().Where(s => s.CampaignId == campaignId).ToList();
            _context.Set<SmsOutbound>().RemoveRange(smsList);
            _context.SaveChanges();
        }

        public SmsOutbound? GetSmsOutboundByRecipientAndIndex(string recipient, int indexSms)
        {
            SmsOutbound? smsOutbound = _context.Set<SmsOutbound>().FirstOrDefault(s => s.Recipient.Value == recipient && s.Index == indexSms);
            return smsOutbound;
        }

        public SmsOutbound? GetSmsOutboundByIndexAndCampaignId(int indexSms, int campaignId)
        {
            SmsOutbound? smsOutbound = _context.Set<SmsOutbound>().FirstOrDefault(s => s.Index == indexSms && s.CampaignId == campaignId);
            return smsOutbound;
        }

        public SmsOutbound? GetSmsOutboundInProgress()
        {
            SmsOutbound? smsOutbound = _context.Set<SmsOutbound>().FirstOrDefault(s => s.Status == SmsStatus.InProgress);
            return smsOutbound;
        }
    }
}
