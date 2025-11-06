namespace SmsMachine.Dashboard.Models
{
    public class CampaignListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
        public int TotalRecipients { get; set; }
        public DateTime CreatedAt { get; set; }
        public CampaignStatus Status { get; set; }

        public enum CampaignStatus { Draft = 0, InProgress = 1, Finished = 2 }
    }
}
