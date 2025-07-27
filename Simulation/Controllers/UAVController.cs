using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Simulation.Common.constants;
using Simulation.Database;
using Simulation.Models;

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

    }
}
