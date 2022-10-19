using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Runtime.Serialization;

namespace FSweeperAPI.Models
{
    public class Player
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public string? name { get; set; }
        public float? score { get; set; }
    }

    public class PlayerList
    {
        public List<Player>? list { get; set; }
    }
}