using SmsMachine.Api.Infrastructure.Utils;
namespace SmsMachine.Api.Models.DTO;

public class CampaignListDTO
{
   
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public int TotalRecipients { get; set; }
    public DateTime CreatedAt { get; set; }
    public CampaignStatus Status { get; set; }

 
}
