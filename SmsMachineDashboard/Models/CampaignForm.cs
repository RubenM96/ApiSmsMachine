using System.ComponentModel.DataAnnotations;

namespace SmsMachine.Dashboard.Models
{
    public class CampaignForm
    {
        [Display(Name = "Nome campagna")]
        [Required(ErrorMessage = "Inserisci il nome della campagna.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Testo SMS")]
        [Required(ErrorMessage = "Inserisci il testo del messaggio.")]

        [StringLength(160, ErrorMessage = "Il testo non può superare 160 caratteri.")]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Destinatari")]
        [Required(ErrorMessage = "Inserisci almeno un numero di telefono.")]
        public string RecipientList { get; set; } = string.Empty;

        [Display(Name = "Notifica di consegna")]
        public bool CampaignNotify { get; set; }

        [Display(Name = "Descrizione")]
        public string? Description { get; set; }
    }
}
