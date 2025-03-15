using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HairBooking__API.Models
{
    public class Store
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("store_name")]
        public string? StoreName { get; set; } = string.Empty;
        [BsonElement("store_address")]
        public string? StoreAddress { get; set; } = string.Empty;
        [BsonElement("store_phone")]
        public string? StorePhone { get; set; } = string.Empty;
        [BsonElement("store_email")]
        public string? StoreEmail { get; set; } = string.Empty;
        [BsonElement("store_bio")]
        public string StoreBio { get; set; } = string.Empty;
        [BsonElement("owner_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = string.Empty;
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;
        [BsonElement("store_slug")]
        public string StoreSlug { get; set; } = string.Empty;
        [BsonElement("store_image_url")]
        public string StoreImageUrl { get; set; } = string.Empty;
        [BsonElement("is_deleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
