using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HairBooking__API.Models
{
    public enum UserRole
    {
        User,
        Admin,
        Owner,
        Staff
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name_customer")]
        public string Name { get; set; } = string.Empty; // Bắt buộc

        [BsonElement("email_customer")]
        public string Email { get; set; } = string.Empty; // Bắt buộc, unique

        [BsonElement("phone_customer")]
        public string? Phone { get; set; } = string.Empty; // Tùy chọn

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; } = string.Empty; // Lưu dạng mã hóa

        [BsonElement("role")]
        public UserRole Role { get; set; } = UserRole.User;

        [BsonElement("address_customer")]
        public string? Address { get; set; } = string.Empty; // Tùy chọn

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("store_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? StoreId { get; set; } = null; // Nullable nếu không phải owner/staff

        [BsonElement("slug_customer")]
        public string SlugCustomer { get; set; } = string.Empty;

        [BsonElement("is_email_verified")]
        public bool IsEmailVerified { get; set; } = false;

        [BsonElement("profile_image_url")]
        public string ProfileImageUrl { get; set; } = string.Empty;
    }
}