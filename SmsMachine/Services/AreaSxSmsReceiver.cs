
namespace SmsMachine.Services
{
    public class AreaSxSmsReceiver : ISmsReceiver
    {
        public void Receive(string index, string recipient, string text, string date)
        {
            //controllare se notifica o sms
            if(IsNotifica(text))
            {
                //Registra Notifica
            }
            else
            {
                //Registra sms
            }
        }

        private bool IsNotifica(string text)
        {
            return text == "STATUS REPORT";
        }
    }
}