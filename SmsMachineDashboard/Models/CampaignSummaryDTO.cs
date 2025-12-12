namespace SmsMachine.Dashboard.Models
{
    public record CampaignSummaryDTO(
    int Total,
    int Draft,
    int InProgress,
    int Finished
);
}
