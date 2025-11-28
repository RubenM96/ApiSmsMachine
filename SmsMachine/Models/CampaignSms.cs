using SmsMachine.Api.Infrastructure.Utils;
using SmsMachine.Api.Models;

namespace SmsMachine.Models
{
    public class CampaignSms
    {
        private CampaignSms()
        {
        }

        public CampaignSms(string title, string text, string recipientList, bool campaignNotify, string? description)
        {
            RecipientList recipients = new RecipientList(recipientList);

            Title = title ?? throw new ArgumentNullException(nameof(title));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            CampaignNotify = campaignNotify;
            CreatedAt = DateTime.UtcNow;
            Status = CampaignStatus.Draft;
            Description = description;
            TotalRecipients = recipients.CalculateTotalRecipients();
            DeliveredCount = 0;
            FailedCount = 0;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool CampaignNotify { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public CampaignStatus Status { get; set; }

        public string? Description { get; set; }
        public int TotalRecipients { get; set; }
        public int DeliveredCount { get; private set; }
        public int FailedCount { get; private set; }

        // metodi di stato 
        public void MarkDraft()
        {
            Status = CampaignStatus.Draft;
        }

        public void MarkInProgress()
        {
            Status = CampaignStatus.InProgress;
        }

        public void MarkFinished()
        {
            Status = CampaignStatus.Finished;
        }

        public bool IsComplete(IReadOnlyCollection<SmsOutbound> smsList)
        {
            bool IsTerminal(SmsStatus status) =>
                status == SmsStatus.Sent ||
                status == SmsStatus.Discard ||
                status == SmsStatus.Failed;

            return smsList.Count > 0 && smsList.All(s => IsTerminal(s.Status));
        }

    }
}
