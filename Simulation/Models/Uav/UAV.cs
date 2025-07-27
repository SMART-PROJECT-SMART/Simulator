using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simulation.Common.Enums;

namespace Simulation.Models.Uav
{
    public class UAV
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int WingId { get; set; }

        public UAVTypes UAVType { get; set; }

        public Dictionary<TelemetryFields,double> TelemetryData { get; set; }

        public string CurrentMissionId { get; set; }
    }
}
