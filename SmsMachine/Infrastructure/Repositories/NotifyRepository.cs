using SmsMachine.Infrastructure.Data;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class NotifyRepository : INotifyRepository
    {
        private readonly SmsDbContext _context;

        public NotifyRepository(SmsDbContext context)
        {
            _context = context;
        }

        public Notify AddNotify(Notify notify)
        {
            _context.Set<Notify>().Add(notify);
            _context.SaveChanges();
            return notify;
        }

        public Notify? GetNotify(int id)
        {
            return _context.Set<Notify>().Find(id);
        }

     
    }
}
