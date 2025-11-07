namespace SmsMachine.Api.Models
{
    public class CampaignFilter
    {
        public string? Title { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public void Normalize()
        {
            Title = string.IsNullOrEmpty(Title) ? null : Title.Trim();
            if (Page < 1) Page = 1;
            if (PageSize <= 0 || PageSize > 100) PageSize = 10;
            if (From.HasValue) From = From.Value.Date;
            if (To.HasValue) To = To.Value.Date;
        }

    }

}
