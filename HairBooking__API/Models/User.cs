using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HairBooking__API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;
        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;
        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;
        [BsonElement("bio")]
        public string Bio { get; set; } = string.Empty;
        [BsonElement("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

    }
}
