using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsMachine.Infrastructure;
using SmsMachine.Services;

namespace SmsMachine.Tests
{
    [TestClass]
    public class SmsSenderTest
    {
        [TestMethod]
        public void SePassoDatiCorrettiParteUnSms()
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(new AreaSxOptions { Password = "SMS1234" });
                services.AddHttpClient<ISmsSender, AreaSxSmsSender>(client =>
                {
                    var baseUrl = "http://192.168.0.101";
                    client.BaseAddress = new Uri(baseUrl);
                });
            });
             
            var app = builder.Build();

            var smsSender = app.Services.GetRequiredService<ISmsSender>();

            //3484041300
            smsSender.SendSms("+393456702752", "Test message from SmsMachine", true);
        }
    }
}
