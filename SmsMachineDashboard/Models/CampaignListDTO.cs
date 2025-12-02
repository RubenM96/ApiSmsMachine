namespace SmsMachine.Dashboard.Models
{
    public record CampaignListDTO(int Id, string Title, int TotalRecipients, DateTime CreatedAt, CampaignStatus Status);

    public enum CampaignStatus { Draft = 0, InProgress = 1, Finished = 2 }
}
