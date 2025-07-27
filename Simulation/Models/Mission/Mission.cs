using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simulation.Models.Mission
{
    public class Mission
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Location Destination { get; set; }

        public int WingId { get; set; }

        public Mission() { }

        public Mission(Location destination, int wingId)
        {
            Destination = destination;
            WingId = wingId;
        }
    }
}
