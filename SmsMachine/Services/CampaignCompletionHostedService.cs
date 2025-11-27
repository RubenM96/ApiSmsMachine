namespace SmsMachine.Api.Services
{
    internal class CampaignCompletionHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CampaignCompletionHostedService> _logger;

        public CampaignCompletionHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<CampaignCompletionHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CampaignCompletionHostedService avviato");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var completionService = scope.ServiceProvider
                        .GetRequiredService<ICampaignCompletionService>();

                    await completionService.CheckCampaignsCompletionAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore nel ciclo di CampaignCompletionHostedService");
                }

                // intervallo tra un controllo e l'altro
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }

            _logger.LogInformation("CampaignCompletionHostedService in arresto");
        }
    }
}
