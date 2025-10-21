namespace SmsMachineDashboard.Models
{
    public class CampaignForm
    {
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string RecipientList { get; set; } = string.Empty;
        public bool CampaignNotify { get; set; }
        public string? Description { get; set; }
    }
}
