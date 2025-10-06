namespace SmsMachine.Infrastructure
{
    public static class AreaSxSmsMachine
    {

        public static class Endpoints
        {
            public const string SendSms = "smssend.cgi"; // Endpoint for sending SMS
            public const string DiscardSms = "smserror.cgi"; // Endpoint for checking discarded SMS
        }
    }

}
