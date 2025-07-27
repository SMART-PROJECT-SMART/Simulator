using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simulation.Common.Enums;

namespace Simulation.Models
{
    public class UAV
    {
        [BsonId]
        [BsonElement("_id"),BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonRepresentation(BsonType.String)]
        public UAVTypes UAVType { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public int CurrentMissionId { get; set; }
    }
}
