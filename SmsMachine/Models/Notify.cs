using System.ComponentModel.DataAnnotations.Schema;

namespace SmsMachine.Models
{
    public class Notify
    {

        private Notify() { }
        public Notify(Recipient recipient, string text, DateTime dateTime, int indexSms, string status, int? smsOutbounsId)
        {
            Recipient = recipient;
            Text = text;
            DateTime = dateTime;
            IndexSms = indexSms;
            Status = status;
            SmsOutbounsId = smsOutbounsId;
        }

        public int Id { get; }
        public Recipient Recipient { get; }
        public string Text { get; }
        public DateTime DateTime { get; }
        public int IndexSms { get; }
        public string Status { get; }
        public int? SmsOutbounsId { get; }

     }
}
