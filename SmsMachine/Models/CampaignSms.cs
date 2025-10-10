namespace SmsMachine.Models
{
    public class CampaignSms
    {
        private CampaignSms()
        {
        }

        public CampaignSms(string title, string text, string recipientList, bool campaignNotify, string? description)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            RecipientList = recipientList ?? throw new ArgumentNullException(nameof(recipientList));
            CampaignNotify = campaignNotify; 
            CreatedAt = DateTime.UtcNow;
            Status = CampaignStatus.Draft;
            Description = description;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string RecipientList { get; set; }
        public bool CampaignNotify { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public CampaignStatus Status { get; set;}
 
        public string? Description { get; set; }

        public enum CampaignStatus
        {
            Draft = 0,
            InProgress = 1,
            Finished = 2
        }


    }
}
