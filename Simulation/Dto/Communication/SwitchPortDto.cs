namespace Simulation.Dto.Communication
{
    public class SwitchPortDto
    {
        public int TailId { get; set; }
        public int TargetPort { get; set; }
        public int UsedPort { get; set; }

        public SwitchPortDto(int tailId, int targetPort, int usedPort)
        {
            TailId = tailId;
            TargetPort = targetPort;
            UsedPort = usedPort;
        }
    }
}
