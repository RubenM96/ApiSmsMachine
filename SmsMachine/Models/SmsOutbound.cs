using SmsMachine.Api.Infrastructure.Utils;

namespace SmsMachine.Models
{
    public class SmsOutbound
    {
        private SmsOutbound()
        {
        }

        public SmsOutbound(Recipient recipient, string text, bool multipart, bool notify, DateTime? sentAt, int? campaignId)
        {
            Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Multipart = multipart;
            Notify = notify;
            SentAt = sentAt ?? throw new ArgumentNullException(nameof(sentAt));
            CampaignId = campaignId;
            Status = SmsStatus.Draft;
        }

        public int? Id { get; set; }
        public Recipient Recipient { get; set; }
        public string Text { get; set; }
        public DateTime? SentAt { get; set; }
        public bool Multipart { get; set; }
        public bool Notify { get; set; }
        public int? Index { get; set; }
        public int? CampaignId { get; set; }
        public SmsStatus Status { get; set; }

        // metodi di stato 
        public void MarkDraft()
        {
            Status = SmsStatus.Draft;
        }
        public void MarkInProgress()
        {
            Status = SmsStatus.InProgress;
        }
        public void MarkSent (int index)
        {
            Status = SmsStatus.Sent;
            Index = index;
            SentAt = DateTime.Now;
        }
        public void MarkDiscarded()
        {
            Status = SmsStatus.Discard;
        }
        public void MarkFailed()
        {
            Status = SmsStatus.Failed; 
        }
    }

}
