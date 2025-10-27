using Microsoft.EntityFrameworkCore;
using SmsMachine.Infrastructure.Data;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class SmsQueueRepository : ISmsQueueRepository
    {
        private readonly SmsDbContext _context;
        public SmsQueueRepository(SmsDbContext context)
        {
            _context = context;
        }

        public SmsQueue AddSmsQueue(SmsQueue smsQueue)
        {
            _context.Set<SmsQueue>().Add(smsQueue);
            _context.SaveChanges();
            return smsQueue;
        }

        public SmsQueue? GetById(int id)
        {
            return _context.Set<SmsQueue>().Find(id);
        }

        public List<SmsQueue>? GetByCampaign(int CampaignId)
        {
            return _context.Set<SmsQueue>()
                            .AsNoTracking()
                            .Where(x => x.CampaignId == CampaignId)
                            .OrderBy(x => x.Id)
                            .ToList();
        }

        public bool Delete(int id)
        {
            var entity = _context.Set<SmsQueue>().Find(id);
            if (entity is null) return false;
            _context.Set<SmsQueue>().Remove(entity);
            _context.SaveChanges();
            return true;
        }

    }
}
