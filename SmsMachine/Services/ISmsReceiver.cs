
namespace SmsMachine.Services
{
    public interface ISmsReceiver
    {
       // void Receive(string code, string recipient, string text, string date);
        void Receive(string code, string recipient, string text, string date, string? sms_id, string? sms_totparts, string? sms_thispart, string? sms_index, string? sms_status);
    
    }
}