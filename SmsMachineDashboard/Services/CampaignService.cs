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

    
}