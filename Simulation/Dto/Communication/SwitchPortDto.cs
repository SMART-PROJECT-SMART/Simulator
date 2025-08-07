using Simulation.Models.Channels;

namespace Simulation.Dto.Communication
{
    public class SwitchPortDto
    {
        public int SourcePort { get; set; }
        public int TargetPort { get; set; }

        public SwitchPortDto() { }

        public SwitchPortDto(int sourcePort, int targetPort)
        {
            SourcePort = sourcePort;
            TargetPort = targetPort;
        }
    }
}
