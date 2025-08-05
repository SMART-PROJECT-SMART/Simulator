using System.Collections;

namespace Simulation.Services.ICD.ICDDirectory
{
    public interface IICDDirectory
    {
        public BitArray DecodeICD(string icdName);
    }
}
