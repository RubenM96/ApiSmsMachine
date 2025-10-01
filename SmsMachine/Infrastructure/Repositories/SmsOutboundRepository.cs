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

        public SmsOutbound? GetSms(int id)
        {
            return _context.Set<SmsOutbound>().Find(id);
        }

    }
}
