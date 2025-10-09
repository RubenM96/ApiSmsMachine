namespace SmsMachine.Services
{
    public interface ICampaignService
    {
        //scheduledAt non è ancora implementato
        void CreateCampaign(string title, string text, string recipientList, string? description = null);
    }
}