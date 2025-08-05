using System.Collections;

namespace Simulation.Services.ICDManagment.ICDDirectory
{
    public interface IICDDirectory
    {
        public BitArray DecodeICD(string icdName);
    }
}
