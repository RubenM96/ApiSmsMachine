namespace SmsMachine.Api.Services
{
    public interface ICheckSmsOutboundToSend
    {
        public Task CheckSmsOutboundInProgress();
    }
}