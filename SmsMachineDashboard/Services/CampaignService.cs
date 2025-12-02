using SmsMachine.Dashboard.Models;
using System.Net;
using System.Web;

public class CampaignService : ICampaignService
{
    private readonly HttpClient _http;

    public CampaignService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CampaignDetails?> GetCampaignIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<CampaignDetails>($"api/campaign/{id}");
    }

    public async Task<bool> SendCampaignAsync(int id)
    {
        // REST "command": POST su /SendCampaign, nessuna logica lato UI.
        var response = await _http.PostAsync($"api/campaign/{id}/SendCampaign", content: null);
        return response.IsSuccessStatusCode;
    }

    //creazione
    public async Task<bool> CreateCampaignAsync(CampaignForm campaignForm)
    {
        var formData = BuildForm(campaignForm);
        var response = await _http.PostAsync("api/campaign/CreateCampaign", formData);
        return response.IsSuccessStatusCode;
    }

    //modifica
    public async Task<bool> UpdateCampaignAsync(int id, CampaignForm campaignForm)
    {
        var formData = BuildForm(campaignForm);
        var response = await _http.PutAsync($"api/campaign/{id}", formData);
        return response.IsSuccessStatusCode;
    }

    //elimina 
    public Task<bool> DeleteCampaignAsync(int id)
    => _http.DeleteAsync($"api/campaign/{id}");

    public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignSearchQuery campaignSearchQuery)
    {
        var qs = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(campaignSearchQuery.Title))
            qs["title"] = campaignSearchQuery.Title;

        if (campaignSearchQuery.From.HasValue)
            qs["from"] = campaignSearchQuery.From.Value.ToString("yyyy-MM-dd");

        if (campaignSearchQuery.To.HasValue)
            qs["to"] = campaignSearchQuery.To.Value.ToString("yyyy-MM-dd");

        qs["page"] = campaignSearchQuery.Page.ToString();
        qs["pageSize"] = campaignSearchQuery.PageSize.ToString();

        var url = $"api/campaign/search?{qs}";

        var result = await _http.GetFromJsonAsync<PagedResult<CampaignListDTO>>(url);

        // in caso di null (API che risponde 204, ecc.)
        return result ?? new PagedResult<CampaignListDTO>
        {
            Items = Array.Empty<CampaignListDTO>(),
            Total = 0,
            Page = campaignSearchQuery.Page,
            PageSize = campaignSearchQuery.PageSize
        };
    }

    //estrarre i recipient di una campagna
    public async Task<string> GetRecipientsByCampaignIdAsync(int campaignId)
    {
        var url = $"api/smsoutbound/GetAllSmsOutboundByCampaignId/{campaignId}";
        var resp = await _http.GetAsync(url);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return string.Empty; // comportamento atteso se non esistono sms

        resp.EnsureSuccessStatusCode();

        var smsList = await resp.Content.ReadFromJsonAsync<List<SmsOutbound>>() ?? new List<SmsOutbound>();

        if (smsList.Count == 0)
            return string.Empty;

        var recipientsString = string.Join(", ",
            smsList
                .Where(s => s.Recipient != null && !string.IsNullOrEmpty(s.Recipient.Value))
                .Select(s => s.Recipient.Value)
        );

        return recipientsString;
    }
   
}