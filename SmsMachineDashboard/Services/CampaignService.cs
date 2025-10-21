using SmsMachineDashboard.Models;
using System.Text;

public class CampaignService
{
    private readonly HttpClient _http;

    public CampaignService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CampaignSms>> GetAllCampaignsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<CampaignSms>>("api/campaign");
        return result ?? new List<CampaignSms>();
    }

    public async Task<CampaignSms?> GetCampaignAsync(int id)
    {
        return await _http.GetFromJsonAsync<CampaignSms>($"api/campaign/{id}/view");
    }

    public async Task<HttpResponseMessage> SendCampaignAsync(int id)
    {
        return await _http.PostAsync($"api/campaign/{id}/send", null);
    }

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
        return await _http.PostAsync("api/campaign", formData);

    }
}