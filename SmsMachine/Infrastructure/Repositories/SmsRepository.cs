using SmsMachine.Infrastructure.Data;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class SmsRepository : ISmsRepository
    {
        private readonly SmsDbContext _context;

        public SmsRepository(SmsDbContext context)
        {
            _context = context;
        }

        public Sms AddSms(Sms sms)
        {
            _context.Set<Sms>().Add(sms);
            _context.SaveChanges();
            return sms;
        }

        public Sms? GetSms(int id)
        {
            return _context.Set<Sms>().Find(id);
        }

    }
}
