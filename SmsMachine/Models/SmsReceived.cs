namespace SmsMachine.Models;

public class SmsReceived
{

    public string? sms_num { get; set; }
    public string? sms_date {  get; set; }
    public string? sms_text { get; set; }
    public string? sms_code { get; set; }
    

    public string? sms_id { get; set; }
    public string? sms_totparts { get; set; }
    public string? sms_thispart { get; set; }

}
