using System.Collections;
using Simulation.Models.ICDModels;

namespace Simulation.Services.ICDManagment.ICDDirectory
{
    public interface IICDDirectory
    {
        public List<ICD> GetAllICDs();
        public void LoadAllICDs();
    }
}
