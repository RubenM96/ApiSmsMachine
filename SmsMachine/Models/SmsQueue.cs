namespace SmsMachine.Models;

public class SmsQueue
{
    private SmsQueue()
    {
    }
    public SmsQueue(Recipient recipient, string text, bool multipart, bool notify, int? campaignId)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Multipart = multipart;
        Notify = notify;
        CampaignId = campaignId;
    }

    public int Id { get; set; }
    public Recipient Recipient { get; set; }
    public string Text { get; set; }
    public bool Multipart { get; set; }
    public bool Notify { get; set; }
    public int? CampaignId { get; set; }

}
