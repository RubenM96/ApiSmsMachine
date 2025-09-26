namespace SmsMachine.Models
{
    public class Notify
    {

        private Notify() { }
        public Notify(Recipient recipient, string text, DateTime dateTime, int indexSms, string status)
        {
            Recipient = recipient;
            Text = text;
            DateTime = dateTime;
            IndexSms = indexSms;
            Status = status;
        }

        public int Id { get; }
        public Recipient Recipient { get; }
        public string Text { get; }
        public DateTime DateTime { get; }
        public int IndexSms { get; }
        public string Status { get; }

    }
}
