using SmsMachine.Dashboard.Models;

public class CampaignService
{
    private readonly HttpClient _http;

    public CampaignService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CampaignDetails>> GetAllCampaignsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<CampaignDetails>>("api/campaign/GetAllCampaigns");
        return result ?? new List<CampaignDetails>();
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
         => _http.PutAsync($"api/campaign/{id}", BuildForm(campaignForm));

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
}