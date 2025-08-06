using Simulation.Models.ICDModels;

namespace Simulation.Services.ICDDirectory
{
    public interface IICDDirectory
    {
        public List<ICD> GetAllICDs();
        public void LoadAllICDs();
    }
}
