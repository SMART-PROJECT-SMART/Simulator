using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Simulation.Common.constants;
using Simulation.Database;
using Simulation.Dto.UAV;
using Simulation.Mappers;
using Simulation.Models;
using Simulation.Ro.UAV;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UAVController : ControllerBase
    {
        private readonly IMongoCollection<UAV> _UAVCollection;

        public UAVController(MongoDbService mongoDbService)
        {
            _UAVCollection = mongoDbService.Database?.GetCollection<UAV>(SimulationConstants.Mongo.Schemas.UAV_SCHEMA);
        }

        [HttpGet("by-uavid/{wingId}")]
        public async Task<ActionResult<UAVRo?>> GetUAVByUAVId(int wingId)
        {
            var filter = Builders<UAV>.Filter.Eq(u => u.WingId, wingId);
            var uav = await _UAVCollection.Find(filter).FirstOrDefaultAsync();
            return uav is not null ? Ok(uav.toRo()) : NotFound();
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateUAV([FromBody] CreateUAVDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uavModel = dto.toModel();
            await _UAVCollection.InsertOneAsync(uavModel);
            return CreatedAtAction(nameof(GetUAVByUAVId),Ok());
        }

        [HttpPatch("update-mission")]
        public async Task<ActionResult<UAVRo?>> UpdateMission([FromBody] UpdateMissionDto dto)
        {
            var update = Builders<UAV>.Update.Set(u => u.CurrentMissionId, dto.MissionId);
            var filter = Builders<UAV>.Filter.Eq(u => u.WingId, int.Parse(dto.WingId));
            var result = await _UAVCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return NotFound();

            var updatedUav = await _UAVCollection.Find(filter).FirstOrDefaultAsync();
            return updatedUav is not null ? Ok() : NotFound();
        }
    }
}