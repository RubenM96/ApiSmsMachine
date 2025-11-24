using System.Text.RegularExpressions;
namespace SmsMachine.Api.Models;

public class RecipientList
{
   
    public RecipientList(string recipientList)
    {
        var recipients = GetRecipientToList(recipientList);
        RegrexRecipient(recipients);

        Recipients = recipients;
    }

    public List<string> Recipients { get; set; }

    public List<string> GetRecipientToList(string recipientList)
    {
        var recipients = recipientList
           .Split(new[] { ',', ';', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
           .Select(r => r.Trim())
           .ToList();
        return recipients;
    }

    public string RegrexRecipient(List<string> recipientList)
    {
        foreach (var recipient in recipientList)
        {
            var isValid = Regex.IsMatch(recipient, @"^\+\d+$");
            if (!isValid)
            {
                throw new ArgumentException($"Il numero {recipient} non è valido. Usa il formato +[prefisso][numero] e solo cifre.");
            }
        }
        return string.Join(",", recipientList);
    }

    public int CalculateTotalRecipients()
    {
        if (Recipients.Count < 1)
            throw new ArgumentException("La lista di destinatari deve contenere almeno un numero di telefono valido.");

        return Recipients.Count();
    }
}
