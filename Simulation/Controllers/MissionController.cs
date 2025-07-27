using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Simulation.Common.constants;
using Simulation.Database;
using Simulation.Dto.Mission;
using Simulation.Dto.UAV;
using Simulation.Mappers;
using Simulation.Models.Mission;
using Simulation.Ro.Mission;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMongoCollection<Mission> _missionCollection;

        public MissionController(MongoDbService mongoDbService)
        {
            _missionCollection = mongoDbService.Database?.GetCollection<Mission>(SimulationConstants.Mongo.Schemas.MISSION_SCHEMA);
        }

        [HttpGet("by-wingid/{wingId}")]
        public async Task<ActionResult<MissionRo>> GetMissionByWingId(int wingId)
        {
            var filter = Builders<Mission>.Filter.Eq(m => m.WingId, wingId);
            var mission = await _missionCollection.Find(filter).FirstOrDefaultAsync();
            return mission is not null ? Ok(mission.ToRo()) : NotFound();
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateMission([FromBody] CreateMissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var missionModel = dto.ToModel();
            await _missionCollection.InsertOneAsync(missionModel);
            return CreatedAtAction(nameof(GetMissionByWingId), new { wingId = missionModel.WingId }, missionModel.ToRo());
        }

        [HttpPatch("update-wingId")]
        public async Task<ActionResult<MissionRo?>> UpdateWingId([FromBody] UpdateMissionDto dto)
        {
            if (!int.TryParse(dto.WingId, out var wingId))
                return BadRequest("Invalid WingId format.");

            var update = Builders<Mission>.Update.Set(m => m.WingId, wingId);
            var filter = Builders<Mission>.Filter.Eq(m => m.WingId, wingId);
            var result = await _missionCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return NotFound();

            var updatedMission = await _missionCollection.Find(filter).FirstOrDefaultAsync();
            return updatedMission is not null ? Ok(updatedMission.ToRo()) : NotFound();
        }
    }
}
