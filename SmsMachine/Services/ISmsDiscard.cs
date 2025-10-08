using SmsMachine.Infrastructure;

namespace SmsMachine.Services
{
    public interface ISmsDiscard
    {
        AreaSxSmsNotSent CheckDiscardSms();
    }
}
