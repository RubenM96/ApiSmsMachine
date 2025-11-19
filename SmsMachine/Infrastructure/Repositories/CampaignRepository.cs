using Microsoft.EntityFrameworkCore;
using SmsMachine.Api.Models;
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

        public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignFilter filter)
        {
            //  il Service chiama filter.Normalize()
            var title = string.IsNullOrWhiteSpace(filter.Title) ? null : filter.Title.Trim();
            DateTime? from = filter.From?.Date;
            DateTime? toInclusive = filter.To?.Date.AddDays(1); // include tutto il giorno

            var campaignSearchQuery = _context.Set<CampaignSms>()
                            .AsNoTracking()
                            .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                campaignSearchQuery = campaignSearchQuery.Where(c => c.Title.Contains(title));

            if (from.HasValue)
                campaignSearchQuery = campaignSearchQuery.Where(c => c.CreatedAt >= from.Value);

            if (toInclusive.HasValue)
                campaignSearchQuery = campaignSearchQuery.Where(c => c.CreatedAt < toInclusive.Value);

            // totale con filtri
            var total = await campaignSearchQuery.CountAsync();

            // ordinamento consigliato: più recenti in alto
            campaignSearchQuery = campaignSearchQuery.OrderByDescending(c => c.CreatedAt);


            var items = await campaignSearchQuery
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(c => new CampaignListDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    Text = c.Text,
                    TotalRecipients = c.TotalRecipients,
                    CreatedAt = c.CreatedAt,
                    Status = c.Status
                })
                .ToListAsync();

            return new PagedResult<CampaignListDTO>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
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
