using System.ComponentModel.DataAnnotations;

namespace SmsMachine.Models
{
    public class SmsSend
    {
        [Required]
        public string? Recipient { get; set; }
        [Required]
        public string? Text { get; set; }
        [Required]
        public bool? Notify { get; set; }
    }
}
