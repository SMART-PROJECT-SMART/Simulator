using Core.Common.Enums;

namespace Simulation.Services.UAVChangeHandlers
{
    public interface IUAVChangeHandlerFactory
    {
        IUAVChangeHandler CreateHandler(CrudOperation operation);
    }
}
