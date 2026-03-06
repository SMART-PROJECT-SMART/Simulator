using Microsoft.AspNetCore.Mvc;
using Simulation.Dto.DeviceManager;
using Simulation.Services.UAVChangeHandlers;
using Simulation.Services.UAVManager;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceManagerWebhookController : ControllerBase
    {
        private readonly IUAVChangeHandlerFactory _handlerFactory;
        private readonly IUAVManager _uavManager;

        public DeviceManagerWebhookController(IUAVChangeHandlerFactory handlerFactory, IUAVManager uavManager)
        {
            _handlerFactory = handlerFactory;
            _uavManager = uavManager;
        }

        [HttpPost("uav-changed")]
        public async Task<ActionResult> UAVChanged([FromBody] UAVChangedNotificationDto notification, CancellationToken cancellationToken)
        {
            IUAVChangeHandler handler = _handlerFactory.CreateHandler(notification.Operation);
            await handler.HandleUAVChangeAsync(notification.TailId, notification.NewTailId, cancellationToken);
            return Ok();
        }

        [HttpPost("uav-ports-changed")]
        public ActionResult UAVPortsChanged([FromBody] UAVPortsChangedNotificationDto notification)
        {
            _uavManager.UpdateChannelPorts(notification.TailId, notification.NewPorts);
            return Ok();
        }
    }
}
