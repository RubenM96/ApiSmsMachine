using SmsMachine.Interfaces;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Repositories
{
    public class NotifyRepository : INotifyRepository
    {
        public Notify AddNotify(Notify notify)
        {
            Console.WriteLine("Aggiunta notifica al database (simulata).");
            Console.WriteLine($"Notifica: {notify.Recipient}, {notify.Text}, {notify.DateTime}");
            throw new NotImplementedException();
        }

        public Notify GetNotify(int id)
        {
            throw new NotImplementedException();
        }


    }
}
