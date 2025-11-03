using Microsoft.EntityFrameworkCore;
using SmsMachine.Api.Models.DTO;
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

        public IEnumerable<CampaignSms> GetAllCampaigns()
        {
            return _context.Set<CampaignSms>().ToList();
        }

        public async Task<IEnumerable<CampaignListDTO>> GetAllCampaignsViews()
        {
            return await _context.Set<CampaignSms>()
                .Select(c => new CampaignListDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    Text = c.Text,
                    TotalRecipients = c.TotalRecipients,
                    CreatedAt = c.CreatedAt,
                    Status = c.Status
                })
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public CampaignSms? GetCampaignId(int id)
        {
            return _context.Set<CampaignSms>().Find(id);
        }


        public CampaignSms UpdateCampaign(CampaignSms campaign)
        {
            _context.Set<CampaignSms>().Update(campaign);
            _context.SaveChanges();
            return campaign;
        }

        public bool DeleteCampaign(int id)
        {
            var entity = _context.Set<CampaignSms>().Find(id);
            if (entity is null) return false;

            _context.Remove(entity);
            _context.SaveChanges();
            return true;
        }



    }
}
