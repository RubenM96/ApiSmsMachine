using SmsMachine.Api.Infrastructure.Utils;

namespace SmsMachine.Api.Models.DTO
{
    public class CampaignProgressDTO
    {
        public int CampaignId { get; set; }
        public CampaignStatus Status { get; set; }

        public int Total { get; set; }
        public int Sent { get; set; }
        public int Discard { get; set; }
        public int Failed { get; set; }
        public int InProgress { get; set; }
        public int Draft { get; set; }

        public double CompletionPercent => Total == 0 ? 0
            : (double)(Sent + Discard + Failed) / Total * 100;
    }
}
