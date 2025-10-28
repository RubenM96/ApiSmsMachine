using Moq;
using SmsMachine.Infrastructure.Utils;
using SmsMachine.Interfaces;
using SmsMachine.Models;


namespace SmsMachine.Tests
{
    [TestClass]
    public class NotifyServiceTest
    {
        // Arrange
        public SmsOutbound? SetupMockSmsOutboundRepository(string recipient, int indexSms)
        {
            var mock = new Mock<ISmsOutboundRepository>();

            DateTime date = DateTime.Now;
            string recipientSms = "+393466270684";
            var smsOutboundTest = new SmsOutbound(new Recipient(recipientSms), "Unit Test", false, true, date);
            smsOutboundTest.Id = 13;
            smsOutboundTest.Index = 12;

            mock.Setup(repo => repo.GetSmsOutboundByRecipientAndIndex(recipientSms, smsOutboundTest.Index.Value)).Returns(smsOutboundTest);

            var result = mock.Object.GetSmsOutboundByRecipientAndIndex(recipient, indexSms);

            return result;
        }

        // Act
        [DataTestMethod]
        [DataRow("0", "+393466270684", "STATUS REPORT TEST", "2024-06-17 10:30:00", 12, "0")]
        public void NotifyTest(string index, string recipient, string text, string date, int indexSms, string status)
        {

            SmsOutbound? smsOutbound = SetupMockSmsOutboundRepository(recipient, indexSms);

            DateTime dateTime = SmsDateParser.ParseDateNotify(date);

            //Assert
            if (smsOutbound == null)
            {
                Console.WriteLine($"Not found SmsOutbound for recipient {recipient} and index {indexSms}");

                var notifyNull = new Notify(new Recipient(recipient), text, dateTime, indexSms, status, null);

                Console.WriteLine($"Info Notify: num {recipient} text: {text}, date {dateTime}, index {indexSms}, status {status}, smsId {smsOutbound}");

            }
            else
            {
                Console.WriteLine("SmsOutbound found");

                var notify = new Notify(new Recipient(recipient), text, dateTime, indexSms, status, smsOutbound.Id);

                Console.WriteLine($"Info Notify: num {recipient} text: {text}, date {dateTime}, index {indexSms}, status {status}, smsId {smsOutbound.Id}");
            }
        }

    }
}
