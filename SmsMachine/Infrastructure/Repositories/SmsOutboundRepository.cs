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

        public SmsOutbound? GetSmsById(int id)
        {
            return _context.Set<SmsOutbound>().Find(id);
        }

        public List<SmsOutbound> GetAllSmsByCampaignId(int campaignId)
        {
            return _context.Set<SmsOutbound>().Where(s => s.CampaignId == campaignId).ToList();
        }

        //metodo per cercare messaggio nel db in base a recipient e indexSms
        public SmsOutbound? GetSmsOutboundByRecipientAndIndex(string recipient, int indexSms)
        {
            SmsOutbound? smsOutbound = _context.Set<SmsOutbound>().FirstOrDefault(s => s.Recipient.Value == recipient && s.Index == indexSms);
            return smsOutbound;
        }

        //metodo per cercare messaggio nel db in base al indexSms e id della campagna
        public SmsOutbound? GetSmsOutboundByIndexAndCampaignId(int indexSms, int campaignId)
        {
            SmsOutbound? smsOutbound = _context.Set<SmsOutbound>().FirstOrDefault(s => s.Index == indexSms && s.CampaignId == campaignId);
            return smsOutbound;
        }
    }
}
