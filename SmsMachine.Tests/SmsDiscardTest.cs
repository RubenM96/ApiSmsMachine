using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsMachine.Infrastructure;
using SmsMachine.Models;
using SmsMachine.Services;
using System;
using static System.Net.Mime.MediaTypeNames;


namespace SmsMachine.Tests
{
    [TestClass]
    public class SmsDiscardTest
    {
        [TestMethod]
        public void RisultatoMessaggiScartati()
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(new AreaSxOptions { Password = "SMS1234" });
                services.AddHttpClient<ISmsDiscard, AreaSxSmsDiscard>(client =>
                {
                    var baseUrl = "http://192.168.0.101";
                    client.BaseAddress = new Uri(baseUrl);
                });
            });

            var app = builder.Build();

            var smsDiscard = app.Services.GetRequiredService<ISmsDiscard>();

            AreaSxSmsNotSent result = (AreaSxSmsNotSent)smsDiscard.DiscardSms();

            Console.WriteLine($"errno:{result.Errno}, errdesc:{result.Errdesc}, SmsTxErrIdx:{result.SmsTxErrIdx}");
        }
    }
}
