using SmsMachine.Api.Infrastructure.Utils;
namespace SmsMachine.Api.Models.DTO;

public record CampaignListDTO(int Id, string Title, int TotalRecipients, DateTime CreatedAt, CampaignStatus Status);
