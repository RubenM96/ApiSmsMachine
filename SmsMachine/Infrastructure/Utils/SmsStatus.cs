namespace SmsMachine.Api.Infrastructure.Utils
{
    public enum SmsStatus
    {
        Draft = 0, // Creato (campagna) ma non ancora programmato per l'invio
        InProgress = 1, // Programmato o in retry (il background service deve lavorarci)
        Sent = 2, // Accettato dalla SMS machine (errno=0)
        Discard = 3, // Scartato in base alle info di errore della macchina (smserror, ecc.)
        Failed = 4 // Errore lato nostro (eccezioni, max retry superato, ecc)
    }

}
