using System.Runtime.CompilerServices;

namespace SmsMachine.Services
{
    public interface ISmsSender
    {
        void SendSms(string Pwd, string num, string text, bool notify);
    }

    internal class SmsSender : ISmsSender
    {
        public void SendSms(string Pwd, string num, string text, bool notify)
        {
           using (var client = new HttpClient())
           {             
                using var request = new HttpRequestMessage(HttpMethod.Post, SmsMachine.endpoint);
           }
        }

    }

    public static class SmsMachine
    {
        public static string endpoint = "http://192.168.0.101/smssend.cgi";          
    }

}
