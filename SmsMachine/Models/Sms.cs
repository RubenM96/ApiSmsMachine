namespace SmsMachine.Models
{
    public class Sms
    {
        private Sms()
        {
        }

        public Sms(Recipient recipient, string text, bool multipart, bool notify, DateTime sentAt)
        {
            Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Multipart = multipart;
            Notify = notify;
            SentAt = sentAt;
        }

        public int Id { get; }
        public Recipient Recipient { get; }
        public string Text { get; }
        public bool Multipart { get; }
        public bool Notify { get; }
        public DateTime? SentAt { get; }
        public int? Index { get; }
        public string? Error { get; }
    }
}
