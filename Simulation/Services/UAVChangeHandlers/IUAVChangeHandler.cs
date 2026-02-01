namespace Simulation.Services.UAVChangeHandlers
{
    public interface IUAVChangeHandler
    {
        Task HandleUAVChangeAsync(int tailId, CancellationToken cancellationToken = default);
    }
}
