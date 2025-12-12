using Microsoft.AspNetCore.Mvc;
using SmsMachine.Dashboard.Models;
using SmsMachine.Dashboard.Services;
using System.Net;
using System.Text;
using System.Text.Json;

public class CampaignService : ICampaignService
{
    private readonly HttpClient _http;

    public CampaignService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CampaignDetails?> GetCampaignIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<CampaignDetails>($"api/campaign/GetCampaign/{id}");
    }

    public async Task<bool> SendCampaignAsync(int id)
    {
        // REST "command": POST su /SendCampaign, nessuna logica lato UI.
        var response = await _http.PostAsync($"api/campaign/SendCampaign/{id}", content: null);
        return response.IsSuccessStatusCode;
    }


    //creazione
    public async Task<CampaignFormResult> CreateCampaignAsync(CampaignForm campaignForm)
    {
        // Serializzo l'oggetto in JSON
        var json = JsonSerializer.Serialize(campaignForm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        string? errorMessage = null;
        string? serverRecipientError = null;

        var response = await _http.PostAsync("api/campaign/CreateCampaign", content);

        // caso ok
        if (response.IsSuccessStatusCode)
        {
            return CampaignFormResult.Ok();
        }
        // caso errore 400 Bad Request con ValidationProblemDetails (errori nel form)
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            try
            {
                var vpd = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

                if (vpd?.Errors != null && vpd.Errors.Count > 0)
                {
                    // errore generale 
                    errorMessage = string.Join(" ", vpd.Errors.SelectMany(kvp => kvp.Value));

                    // errore specifico su RecipientList
                    if (vpd.Errors.TryGetValue(nameof(campaignForm.RecipientList), out var recipientErrors))
                        serverRecipientError = string.Join(" ", recipientErrors);
                    else
                        serverRecipientError ??= errorMessage;
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    errorMessage = string.IsNullOrWhiteSpace(body) ? "Richiesta non valida." : body;
                    serverRecipientError ??= errorMessage;
                }
            }
            catch
            {
                var body = await response.Content.ReadAsStringAsync();
                errorMessage = string.IsNullOrWhiteSpace(body) ? "Richiesta non valida." : body;
                serverRecipientError ??= errorMessage;
            }
            return CampaignFormResult.Fail(errorMessage, serverRecipientError);
        }
        // altri errori (500, ecc.)
        var rawBody = await response.Content.ReadAsStringAsync();
        errorMessage = string.IsNullOrWhiteSpace(rawBody)
            ? $"Errore server ({(int)response.StatusCode})."
            : rawBody;

        return CampaignFormResult.Fail(errorMessage);
    }


    public async Task<CampaignFormResult> UpdateCampaignAsync(int id, CampaignForm campaignForm)
    {
        string? errorMessage = null;
        string? serverRecipientError = null;

        var json = JsonSerializer.Serialize(campaignForm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PutAsync($"api/campaign/UpdateCampaign/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            return CampaignFormResult.Ok();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            try
            {
                var vpd = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (vpd?.Errors != null && vpd.Errors.Count > 0)
                {
                    var all = vpd.Errors.SelectMany(kvp => kvp.Value).ToArray();
                    errorMessage = string.Join(" ", all);

                    if (vpd.Errors.TryGetValue(nameof(campaignForm.RecipientList), out var recErrs))
                        serverRecipientError = string.Join(" ", recErrs);
                    else
                        serverRecipientError ??= errorMessage;
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    errorMessage = string.IsNullOrWhiteSpace(body) ? "Richiesta non valida." : body;
                    serverRecipientError ??= errorMessage;
                }
            }
            catch
            {
                var body = await response.Content.ReadAsStringAsync();
                errorMessage = string.IsNullOrWhiteSpace(body) ? "Richiesta non valida." : body;
                serverRecipientError ??= errorMessage;
            }

            return CampaignFormResult.Fail(errorMessage, serverRecipientError);
        }

        var rawBody = await response.Content.ReadAsStringAsync();
        errorMessage = string.IsNullOrWhiteSpace(rawBody)
            ? $"Errore server ({(int)response.StatusCode})."
            : rawBody;

        return CampaignFormResult.Fail(errorMessage);
    }

    //elimina 
    public async Task<bool> DeleteCampaignAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/campaign/DeleteCampaign/{id}");
        return response.IsSuccessStatusCode;
    }

    //Lista
    public async Task<PagedResult<CampaignListDTO>> SearchAsync(CampaignSearchQuery campaignSearch)
    {
        var response = await _http.PostAsJsonAsync("api/campaign/search", campaignSearch);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<CampaignListDTO>>();

        return result ?? new PagedResult<CampaignListDTO>
        {
            Items = Array.Empty<CampaignListDTO>(),
            Total = 0,
            Page = campaignSearch.Page,
            PageSize = campaignSearch.PageSize
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

    public async Task<CampaignProgressDTO?> GetProgressAsync(int campaignId)
    {
        return await _http.GetFromJsonAsync<CampaignProgressDTO>($"api/campaign/GetProgress/{campaignId}");
    }
    public async Task<CampaignSummaryDTO?> GetSummaryAsync()
    {
        return await _http.GetFromJsonAsync<CampaignSummaryDTO>("api/campaign/summary");
    }

}