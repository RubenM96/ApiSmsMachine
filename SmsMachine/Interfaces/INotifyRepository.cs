using SmsMachine.Models;

namespace SmsMachine.Interfaces
{
    public interface INotifyRepository
    {
        Notify AddNotify(Notify notify);
        Notify GetNotify(int id);
    }
}
