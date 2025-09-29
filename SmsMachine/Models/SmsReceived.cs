namespace SmsMachine.Models;

public class SmsReceived
{
    public string? sms_num { get; set; }
    public string? sms_date {  get; set; }
    public string? sms_text { get; set; }
    public string? sms_code { get; set; }


    //atributi per sms esteso
    public string? sms_id { get; set; } //necessario per la notifica
    public string? sms_totparts { get; set; }
    public string? sms_thispart { get; set; }


    //atributi per la notifica
    public string? sms_status { get; set; }

}
