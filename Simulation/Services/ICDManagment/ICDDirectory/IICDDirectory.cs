using Simulation.Models.ICD;
using System.Collections;

namespace Simulation.Services.ICDManagment.ICDDirectory
{
    public interface IICDDirectory
    {
        public List<ICD> GetAllICDs();
    }
}
