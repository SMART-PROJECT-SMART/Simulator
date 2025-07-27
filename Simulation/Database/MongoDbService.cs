using MongoDB.Driver;
using Simulation.constants;

namespace Simulation.Database
{
    public sealed class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;

        private MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;

            string connectionString = _configuration.GetConnectionString(SimulationConstants.Configuration.DB_CONNECTION);
            MongoUrl mongoUrl = MongoUrl.Create(connectionString);
            MongoClient mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        private static MongoDbService _instance;

        public static MongoDbService GetInstance(IConfiguration configuration)
        {
            _instance ??= new MongoDbService(configuration);
            return _instance;
        }
    }
}
