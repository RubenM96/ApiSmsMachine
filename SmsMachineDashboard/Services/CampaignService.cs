using Microsoft.AspNetCore.Mvc;
using SmsMachine.Dashboard.Models;
using SmsMachine.Dashboard.Services;
using System.Net;
using System.Text;
using System.Text.Json;
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
        // Serializzo l'oggetto in JSON
        var json = JsonSerializer.Serialize(campaignForm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        string? successMessage = null;
        string? errorMessage = null;
        string? serverRecipientError = null;

        var response = await _http.PostAsync("api/campaign/CreateCampaign", content);

        if (response.IsSuccessStatusCode)
        {
            successMessage = "Campagna creata con successo.";
            // reset del form
            campaignForm = new CampaignForm();
        }
        else if ((int)response.StatusCode == 400)
        {
            try
            {
                // Provo a leggere la risposta come ValidationProblemDetails
                var vpd = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

                if (vpd?.Errors != null && vpd.Errors.Count > 0)
                {
                    // Messaggio di errore generale
                    errorMessage = string.Join(" ", vpd.Errors.SelectMany(kvp => kvp.Value));

                    // Errore specifico su RecipientList, se presente
                    if (vpd.Errors.TryGetValue(nameof(campaignForm.RecipientList), out var recipientErrors))
                        serverRecipientError = string.Join(" ", recipientErrors);
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                    serverRecipientError ??= errorMessage;
                }
            }
            catch
            {
                // Se la deserializzazione fallisce, mostro il body grezzo
                errorMessage = await response.Content.ReadAsStringAsync();
                serverRecipientError ??= errorMessage;
            }
        }
        else
        {
            // Altri errori generici
            var body = await response.Content.ReadAsStringAsync();
            errorMessage = string.IsNullOrWhiteSpace(body)
                ? $"Errore server ({(int)response.StatusCode})."
                : body;
        }

        // Qui puoi eventualmente loggare o mostrare successMessage, errorMessage, serverRecipientError
        // Es: Debug.WriteLine(successMessage ?? errorMessage);
        return response.IsSuccessStatusCode;
    }

    //modifica
    public async Task<bool> UpdateCampaignAsync(int id, CampaignForm campaignForm)
    {

        string? errorMessage =null;          // alert rosso in alto
        string? successMessage = null;        // alert verde in alto
        string? serverRecipientError = null;  // messaggio specifico sotto "Destinatari"
        var json = JsonSerializer.Serialize(campaignForm);

        // Crea il contenuto HTTP con il tipo 'application/json'
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PutAsync($"api/campaign/UpdateCampaign/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            successMessage = "Campagna aggiornata con successo.";
        }
        else if ((int)response.StatusCode == 400)
        {
            // ValidationProblemDetails dal server
            try
            {
                var vpd = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ValidationProblemDetails>();
                if (vpd?.Errors != null && vpd.Errors.Count > 0)
                {
                    var all = vpd.Errors.SelectMany(kvp => kvp.Value).ToArray();
                    errorMessage = string.Join(" ", all);

                    if (vpd.Errors.TryGetValue(nameof(campaignForm.RecipientList), out var recErrs))
                        serverRecipientError = string.Join(" ", recErrs);
                    else
                        serverRecipientError ??= errorMessage; // fallback sotto il campo
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
        }
        else
        {
            var body = await response.Content.ReadAsStringAsync();
            errorMessage = string.IsNullOrWhiteSpace(body)
                ? $"Errore server ({(int)response.StatusCode})."
                : body;
        }



        return response.IsSuccessStatusCode;
    }

    //elimina 
    public async Task<bool> DeleteCampaignAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/campaign/DeleteCampaign/{id}");
        return response.IsSuccessStatusCode;
    }

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

    public async Task<CampaignProgressDTO?> GetProgressAsync(int campaignId)
    {
        return await _http.GetFromJsonAsync<CampaignProgressDTO>($"api/campaign/GetProgress/{campaignId}");
    }

}