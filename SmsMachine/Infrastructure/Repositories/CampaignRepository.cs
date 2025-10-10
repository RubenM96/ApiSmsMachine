using SmsMachine.Infrastructure.Data;
using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {

        private readonly SmsDbContext _context;

        public CampaignRepository(SmsDbContext context)
        {
            _context = context;
        }

        public CampaignSms AddCampaign(CampaignSms campaign)
        {
            _context.Set<CampaignSms>().Add(campaign);
            _context.SaveChanges();
            return campaign;
        }

        public CampaignSms? GetCampaignId(int id)
        {
            return _context.Set<CampaignSms>().Find(id);
        }



    }
}
