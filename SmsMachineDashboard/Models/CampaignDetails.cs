namespace SmsMachineDashboard.Models
{
    public class CampaignDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string RecipientList { get; set; }
        public bool CampaignNotify { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public CampaignStatus Status { get; set; }

        public string? Description { get; set; }

        public enum CampaignStatus
        {
            Draft = 0,
            InProgress = 1,
            Finished = 2
        }
    }
}
