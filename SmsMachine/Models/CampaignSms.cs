using SmsMachine.Api.Infrastructure.Utils;
using System.Text.RegularExpressions;

namespace SmsMachine.Models
{
    public class CampaignSms 
    {
        private CampaignSms()
        {
        }

        public CampaignSms(string title, string text, string recipientList, bool campaignNotify, string? description)
        {
            var recipients = GetRecipientToList(recipientList);

            Title = title ?? throw new ArgumentNullException(nameof(title));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            RecipientList = RegrexRecipient(recipients) ?? throw new ArgumentNullException(nameof(recipientList));
            CampaignNotify = campaignNotify;
            CreatedAt = DateTime.UtcNow;
            Status = CampaignStatus.Draft;
            Description = description;
            TotalRecipients = CalculateTotalRecipients(recipients);
            DeliveredCount = 0;
            FailedCount = 0;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string RecipientList { get; set; }
        public bool CampaignNotify { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public CampaignStatus Status { get; set; }

        public string? Description { get; set; }
        public int TotalRecipients { get; set; }
        public int DeliveredCount { get; private set; }
        public int FailedCount { get; private set; }



        // helper per aggiornare i contatori e controllare la lista di numeri
        public List<string> GetRecipientToList(string recipientList)
        {
            var recipients = recipientList
               .Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(r => r.Trim())
               .ToList();
            return recipients;
        }

        public string RegrexRecipient(List<string> recipientList)
        {
            foreach (var recipient in recipientList)
            {
                var isValid = Regex.IsMatch(recipient, @"^\+\d+$");
                if (!isValid)
                {
                    throw new ArgumentException($"Phone number {recipient} contains invalid characters", nameof(recipient));
                }
            }
            return string.Join(",", recipientList);
        }

        public int CalculateTotalRecipients(List<string> recipientList)
        {
            if (recipientList.Count < 1)
                throw new ArgumentException("The recipient list must contain at least one valid phone number.");

            return recipientList.Count();
        }

        public void IncDelivered() => DeliveredCount++;
        public void IncFailed() => FailedCount++;
        public bool IsComplete() => (DeliveredCount + FailedCount) >= TotalRecipients && TotalRecipients > 0;
    }
}
