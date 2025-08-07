using Simulation.Models.Channels;

namespace Simulation.Dto.ICD
{
    public class StartUpLoadDto
    {
        public string ICDName { get; set; }
        public Channel Channel { get; set; }

        public StartUpLoadDto(string icdName, Channel channel)
        {
            ICDName = icdName;
            Channel = channel;
        }
    }
}
