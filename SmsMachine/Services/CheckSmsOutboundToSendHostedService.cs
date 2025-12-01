namespace SmsMachine.Api.Services;

internal class CheckSmsOutboundToSendHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CheckSmsOutboundToSendHostedService> _logger;

    public CheckSmsOutboundToSendHostedService(IServiceScopeFactory scopeFactory, ILogger<CheckSmsOutboundToSendHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var logic = scope.ServiceProvider.GetRequiredService<ICheckSmsOutboundToSend>();
                await logic.CheckSmsOutboundInProgress();
            }
            catch (Exception)
            {
                _logger.LogError("Error checking SMS outbound to send.");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
