using SmsMachine.Infrastructure.Data;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class SmsInboundRepository : ISmsInboundRepository
    {
        private readonly SmsDbContext _context;
        public SmsInboundRepository(SmsDbContext context) => _context = context;

        public SmsInbound AddSmsInbound(SmsInbound smsInbound)
        {
            _context.Set<SmsInbound>().Add(smsInbound);
            _context.SaveChanges();
            return smsInbound;
        }
        public SmsInbound? GetSmsInbound(int id)
        {
            return _context.Set<SmsInbound>().Find(id);
        }

    }

}
