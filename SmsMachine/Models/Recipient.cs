using Microsoft.EntityFrameworkCore;

namespace SmsMachine.Models
{

    //controllare questo
    [Owned]
    public record Recipient
    {

        public Recipient(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            
            if (!IsValid(value))
                throw new ArgumentException("Invalid phone number format", nameof(value));
        }

        public string Value { get; }

        public static bool IsValid(string value)
        {
            return value.StartsWith("+");
        }

        public override string ToString() => Value;
    }


    /*
    public record Money
    {
        public Money(double amount)
        {
            Amount = amount;
        }

        public double Amount { get; }

        public override string ToString() => Amount.ToString("F2");
        public static Money operator +(Money m1, Money m2) => new Money(m1.Amount + m2.Amount);
    }
    */

}
