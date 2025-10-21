using SmsMachineDashboard.Models;

public class CampaignService
{
    private readonly HttpClient _http;

    public CampaignService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CampaignForm>> GetAllCampaignsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<CampaignForm>>("api/campaign");
        return result ?? new List<CampaignForm>();
    }

    public async Task<CampaignForm?> GetCampaignAsync(int id)
    {
        return await _http.GetFromJsonAsync<CampaignForm>($"api/campaign/{id}/view");
    }

    public async Task<HttpResponseMessage> SendCampaignAsync(int id)
    {
        return await _http.PostAsync($"api/campaign/{id}/send", null);
    }

    public async Task<HttpResponseMessage> CreateCampaignAsync(CampaignForm form)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(form.Title), nameof(form.Title) },
            { new StringContent(form.Text), nameof(form.Text) },
            { new StringContent(form.RecipientList), nameof(form.RecipientList) },
            { new StringContent(form.CampaignNotify.ToString()), nameof(form.CampaignNotify) },
        };

        if (!string.IsNullOrEmpty(form.Description))
            content.Add(new StringContent(form.Description), nameof(form.Description));

        return await _http.PostAsync("api/campaign", content);
    }

    public async Task<HttpResponseMessage> UpdateCampaignAsync(int id, CampaignForm form)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(form.Title), nameof(form.Title) },
            { new StringContent(form.Text), nameof(form.Text) },
            { new StringContent(form.RecipientList), nameof(form.RecipientList) },
            { new StringContent(form.CampaignNotify.ToString()), nameof(form.CampaignNotify) },
        };

        if (!string.IsNullOrEmpty(form.Description))
            content.Add(new StringContent(form.Description), nameof(form.Description));

        return await _http.PutAsync($"api/campaign/{id}", content);
    }
}