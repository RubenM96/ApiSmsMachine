namespace SmsMachine.Api.Models.DTO
{
    public record CampaignSummaryDTO(
    int Total,
    int Draft,
    int InProgress,
    int Finished
    );

}
