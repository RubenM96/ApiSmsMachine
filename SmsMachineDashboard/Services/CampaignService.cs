using SmsMachine.Dashboard.Models;
using System.Net;
using System.Web;

public class CampaignService
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

    public async Task<HttpResponseMessage> SendCampaignAsync(int id)
    {
        return await _http.PostAsync($"api/campaign/{id}/SendCampaign", null);
    }

    //creazione
    public async Task<HttpResponseMessage> CreateCampaignAsync(CampaignForm campaignForm)
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(campaignForm.Title), "Title" },
            { new StringContent(campaignForm.Text), "Text" },
            { new StringContent(campaignForm.RecipientList), "RecipientList" },
            { new StringContent(campaignForm.CampaignNotify.ToString()), "CampaignNotify" }
        };
        if (!string.IsNullOrEmpty(campaignForm.Description))
        {
            formData.Add(new StringContent(campaignForm.Description), "Description");
        }

        return await _http.PostAsync("api/campaign/CreateCampaign", formData);
    }

    //modifica
    public Task<HttpResponseMessage> UpdateCampaignAsync(int id, CampaignForm campaignForm)
    {
        var response = _http.PutAsync($"api/campaign/{id}", BuildForm(campaignForm));
        return response;
    }

    private static MultipartFormDataContent BuildForm(CampaignForm campaignForm)
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(campaignForm.Title ?? string.Empty), "Title" },
            { new StringContent(campaignForm.Text ?? string.Empty), "Text" },
            { new StringContent(campaignForm.RecipientList ?? string.Empty), "RecipientList" },
            { new StringContent(campaignForm.CampaignNotify.ToString()), "CampaignNotify" }
        };
        if (!string.IsNullOrWhiteSpace(campaignForm.Description))
            formData.Add(new StringContent(campaignForm.Description), "Description");
        return formData;
    }

    //elimina 
    public Task<HttpResponseMessage> DeleteCampaignAsync(int id)
    => _http.DeleteAsync($"api/campaign/{id}");

    public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignSearchQuery campaignSearchQuery)
    {
        //querystring
        var qs = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrWhiteSpace(campaignSearchQuery.Title)) qs["title"] = campaignSearchQuery.Title;
        if (campaignSearchQuery.From.HasValue) qs["from"] = campaignSearchQuery.From.Value.ToString("yyyy-MM-dd");
        if (campaignSearchQuery.To.HasValue) qs["to"] = campaignSearchQuery.To.Value.ToString("yyyy-MM-dd");
        qs["page"] = (campaignSearchQuery.Page <= 0 ? 1 : campaignSearchQuery.Page).ToString();
        qs["pageSize"] = (campaignSearchQuery.PageSize <= 0 ? 10 : campaignSearchQuery.PageSize).ToString();

        var url = $"api/campaign/search?{qs}";
        var res = await _http.GetFromJsonAsync<PagedResult<CampaignListDTO>>(url);
        return res ?? new PagedResult<CampaignListDTO>
        {
            Items = Array.Empty<CampaignListDTO>(),
            Total = 0,
            Page = campaignSearchQuery.Page <= 0 ? 1 : campaignSearchQuery.Page,
            PageSize = campaignSearchQuery.PageSize <= 0 ? 10 : campaignSearchQuery.PageSize
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

        if (smsList.Count == 0) return string.Empty;

        var recipientsString = string.Join(", ",
            smsList
                .Where(s => s.Recipient != null && !string.IsNullOrEmpty(s.Recipient.Value))
                .Select(s => s.Recipient.Value)
        );

        return recipientsString;
    }
}