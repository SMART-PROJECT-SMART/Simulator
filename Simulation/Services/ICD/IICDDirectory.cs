using System.Collections;

namespace Simulation.Services.ICD
{
    public interface IICDDirectory
    {
        public BitArray DecodeICD(string icdName);
    }
}
