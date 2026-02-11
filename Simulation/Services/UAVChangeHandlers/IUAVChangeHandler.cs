namespace Simulation.Services.UAVChangeHandlers
{
    public interface IUAVChangeHandler
    {
        Task HandleUAVChangeAsync(int tailId, int? newTailId = null, CancellationToken cancellationToken = default);
    }
}
