using MongoDB.Driver;
using Simulation.Common.constants;

namespace Simulation.Database
{
    public sealed class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;

            string connectionString = _configuration.GetConnectionString(SimulationConstants.Configuration.DB_CONNECTION);
            MongoUrl mongoUrl = MongoUrl.Create(connectionString);
            MongoClient mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase? Database => _database;
    }
}
