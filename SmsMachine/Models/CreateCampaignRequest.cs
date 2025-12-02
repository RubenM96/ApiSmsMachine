using System.ComponentModel.DataAnnotations;

namespace SmsMachine.Models
{
    public class CreateCampaignRequest
    {

        [Required]
        public string Title { get; set; }

        [Required]
        [StringLength(160, ErrorMessage = "Il testo non può superare 160 caratteri.")]
        public string Text { get; set; }

        [Required]
        public string RecipientList { get; set; }

        [Required]
        public bool CampaignNotify { get; set; }

        public string? Description { get; set; }
    }
}
