using Serilog;

public static class LoggingConfig
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("Logs/app.log")

            // SMS inviati
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e =>
                    e.Properties.ContainsKey("SmsType") &&
                    e.Properties["SmsType"].ToString() == "\"Sent\"")
                .WriteTo.File("Logs/sms_sent.log", rollingInterval: RollingInterval.Day))
            
            // Notify non inviati
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e =>
                    e.Properties.ContainsKey("Notify") &&
                    e.Properties["Notify"].ToString() == "\"NotifyNotSent\"")
                .WriteTo.File("Logs/notify_not_sent.log", rollingInterval: RollingInterval.Day))

            // Area Sx Sender Response
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e =>
                    e.Properties.ContainsKey("AresSx") &&
                    e.Properties["AresSx"].ToString() == "\"AreaSxSenderResponse\"")
                .WriteTo.File("Logs/areasx_sender_responde.log", rollingInterval: RollingInterval.Day))


            // SMS scartati
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e =>
                    e.Properties.ContainsKey("AresSx") &&
                    e.Properties["AresSx"].ToString() == "\"AreaSxDiscardSms\"")
                .WriteTo.File("Logs/discard_sms.log", rollingInterval: RollingInterval.Day))

            .CreateLogger();
    }
}

