using SmsMachine.Infrastructure;

namespace SmsMachine.Tests
{
    internal class SmsDiscardResult : AreaSxSmsNotSent
    {
        public string SmsTxErrIdx { get; set; }
    }
}