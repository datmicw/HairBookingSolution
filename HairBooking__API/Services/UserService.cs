using HairBooking__API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace HairBooking__API.Services

{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDBSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDBSettings:DatabaseName"]);
            _users = database.GetCollection<User>(config["MongoDBSettings:CollectionName"]);
        }
        // lấy danh sách Users
        public async Task<List<User>> GetAllUsers() => await _users.Find(user => true).ToListAsync();
        // thêm User mới
        public async Task CreateUser(User user) => await _users.InsertOneAsync(user);
        // sửa user
        public async Task<bool> UpdateUser(string id, User updatedUser)
        {
            var result = await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
            return result.ModifiedCount > 0;
        }
        // xóa User
        public async Task<bool> DeleteUser(string id)
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id);
            return result.DeletedCount > 0;
        }
        // lấy User theo ID
        public async Task<User?> GetUserById(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }
        public async Task<int> CountAdmins()
        {
            return (int)await _users.CountDocumentsAsync(user => user.Role == "admin");
        }


    }
}