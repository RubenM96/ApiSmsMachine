namespace SmsMachine.Api.Services;

internal class CheckSmsOutboundToSendHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CheckSmsOutboundToSendHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var logic = scope.ServiceProvider.GetRequiredService<ICheckSmsOutboundToSend>();

            await logic.CheckSmsOutboundInProgress();

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

}
