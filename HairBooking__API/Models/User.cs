using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HairBooking__API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name_customer")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email_customer")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone_customer")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = "user";

        [BsonElement("address_customer")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("store_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? StoreId { get; set; }
         [BsonElement("slug_customer")]
        public string SlugCustomer { get; set; } = string.Empty;
    }
}
