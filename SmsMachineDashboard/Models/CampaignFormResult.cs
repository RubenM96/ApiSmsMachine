namespace SmsMachine.Dashboard.Models
{
    public class CampaignFormResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public string? RecipientError { get; init; }

        public static CampaignFormResult Ok() => new() { Success = true };

        public static CampaignFormResult Fail(string? errorMessage, string? recipientError = null)
            => new() { Success = false, ErrorMessage = errorMessage, RecipientError = recipientError };
    }
}
