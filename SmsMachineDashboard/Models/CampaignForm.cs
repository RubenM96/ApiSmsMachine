using System.ComponentModel.DataAnnotations;

namespace SmsMachineDashboard.Models
{
    public class CampaignForm
    {

        public CampaignForm(){}
        public CampaignForm(string title, string text, string recipientlist, bool campaignnotify, string? description)
        {
            Title = title;
            Text = text;
            RecipientList = recipientlist;
            CampaignNotify = campaignnotify;
            Description = description;
        }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(160, ErrorMessage = "Il testo non può superare 160 caratteri.")]
        public string Text { get; set; } = string.Empty;

        [Required]
        public string RecipientList { get; set; } = string.Empty;

        [Required]
        public bool CampaignNotify { get; set; }
        
        public string? Description { get; set; }
    }
}
