using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SmsMachine.Infrastructure.Utils
{
    public static class SmsDateParser
    {
        // Formato fisso: "dd-MM-yy HH:mm:ss +GMT:00"
        public static DateTime ParseDateFromSmsMachine(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new FormatException("La data dell'SMS è vuota.");

            // rimuove il suffisso " +GMT:00" se presente
            var ix = input.IndexOf(" +GMT:", StringComparison.OrdinalIgnoreCase);
            var core = ix >= 0 ? input[..ix] : input;

            return DateTime.ParseExact(
                core.Trim(),
                "dd-MM-yy HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            );
        }

        public static DateTime ParseDateNotify(string date)
        {          
            date = date.Substring(0, 19);

            DateTime dateTime = DateTime.ParseExact(
                    date,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None
                );

            return dateTime;
        }


    }
}
