using System;
using static SmsMachine.Dashboard.Models.CampaignDetails;

namespace SmsMachine.Dashboard.Models
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

        // Completamento basato sugli stati terminali (Sent + Discard + Failed)
        public double CompletionPercent =>
            Total == 0
                ? 0
                : (double)(Sent + Discard + Failed) / Total * 100;
    }
}
