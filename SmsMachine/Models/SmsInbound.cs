
namespace SmsMachine.Models
{
    public class SmsInbound
    {
        private SmsInbound()
        {
        }

        public SmsInbound(Recipient recipient, string text, bool multipart, DateTime receivedAt)
        {
            Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Multipart = multipart;
           // Notify = notify;
            ReceivedAt = receivedAt;
        }
        public int Id { get; }
        public Recipient Recipient { get; }
        public string Text { get; }
        public bool Multipart { get; }
        public DateTime ReceivedAt { get; }     
        public string? Error { get; }

    }
}
