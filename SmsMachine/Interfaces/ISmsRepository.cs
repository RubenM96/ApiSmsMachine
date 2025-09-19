using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface ISmsRepository
    {
        Sms AddSms(Sms sms);
        Sms? GetSms(int id);
    }
}
