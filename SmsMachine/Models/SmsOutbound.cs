using System.ComponentModel.DataAnnotations.Schema;

namespace SmsMachine.Models
{
    public class SmsOutbound
    {
        private SmsOutbound()
        {
        }

        public SmsOutbound(Recipient recipient, string text, bool multipart, bool notify, DateTime sentAt)
        {
            Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Multipart = multipart;
            Notify = notify;
            SentAt = sentAt;
        }

        public int Id { get; set; }
        public Recipient Recipient { get; }
        public string Text { get; }
        public DateTime SentAt { get; }
        public bool Multipart { get; }
        public bool Notify { get; }
        public int? Index { get; set;}
        public int? CampaignId { get; set; }
        public string? Error { get; }


    }
}
