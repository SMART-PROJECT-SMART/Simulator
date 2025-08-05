using Simulation.Models.ICD;

namespace Simulation.Services.ICD.Interfaces
{
    public interface IICDGenerationStrategy
    {
        List<ICDItem> GenerateICDItems();
        Task SaveICDAsync(List<ICDItem> icdItems, string filename);
    }
}
