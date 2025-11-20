namespace SmsMachine.Dashboard.Models
{


    public class SmsOutbound
    {
        public int? Id { get; set; }
        public Recipient Recipient { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool Multipart { get; set; }
        public bool Notify { get; set; }
        public int? Index { get; set; }
        public int? CampaignId { get; set; }
        public SmsStatus Status { get; set; }

    }
    public class Recipient
    {
        public string Value { get; set; } = string.Empty;
    }
    public enum SmsStatus
    {
        Draft = 0,
        InProgress = 1,
        Sent = 2,
        Discard = 3,
        Failed = 4
    }
}

