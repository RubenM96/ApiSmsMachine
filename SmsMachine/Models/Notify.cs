namespace SmsMachine.Models
{
    public class Notify
    {

        private Notify() { }
        public Notify( Recipient recipient, string text, DateTime dateTime)
        {
            Recipient = recipient;
            Text = text;
            DateTime = dateTime;
        }
       
        public int Id { get; set; }
        public Recipient Recipient { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        
    }
}
