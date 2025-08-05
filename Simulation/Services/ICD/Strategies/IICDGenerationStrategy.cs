using Simulation.Models.ICD;

namespace Simulation.Services.ICD.Strategies
{
    public interface IICDGenerationStrategy
    {
        List<ICDItem> GenerateICDItems();
        Task SaveICDAsync(List<ICDItem> icdItems, string filename);
    }
}
