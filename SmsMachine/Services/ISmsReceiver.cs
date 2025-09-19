
namespace SmsMachine.Services
{
    public interface ISmsReceiver
    {
        void Receive(string index, string recipient, string text, string date);
    }
}