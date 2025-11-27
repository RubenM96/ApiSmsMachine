namespace SmsMachine.Api.Infrastructure.Utils;

public enum CampaignStatus
{
    Draft = 0, // Campagna solo creata 
    InProgress = 1, // L'utente ha premuto invia 
    Finished = 2 // Tutti gli SMS della campagna sono in uno stato terminale ( Sent, Discard, Failed ) 
}
