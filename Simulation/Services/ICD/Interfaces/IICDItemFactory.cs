using Simulation.Common.Enums;
using Simulation.Models.ICD;

namespace Simulation.Services.ICD.Interfaces
{
    public interface IICDItemFactory
    {
        ICDItem CreateItem(TelemetryFields field, int bitOffset, int bitLength);
    }
}
