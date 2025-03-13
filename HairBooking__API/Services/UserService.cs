using HairBooking__API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HairBooking__API.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDBSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDBSettings:DatabaseName"]);
            _users = database.GetCollection<User>("Users");
        }

        // Lấy danh sách Users
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                return await _users.Find(user => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách người dùng: {ex.Message}");
                return new List<User>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        // Thêm User mới
        public async Task<bool> CreateUser(User user)
        {
            try
            {
                await _users.InsertOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo user: {ex.Message}");
                return false;
            }
        }

        // Cập nhật User theo ID
        public async Task<bool> UpdateUser(string id, User updatedUser)
        {
            try
            {
                var result = await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật user: {ex.Message}");
                return false;
            }
        }

        // Xóa User theo ID
        public async Task<bool> DeleteUser(string id)
        {
            try
            {
                var result = await _users.DeleteOneAsync(user => user.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa user: {ex.Message}");
                return false;
            }
        }

        // Lấy User theo ID
        public async Task<User?> GetUserById(string id)
        {
            try
            {
                return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy user theo ID: {ex.Message}");
                return null;
            }
        }

        // Lấy User theo email
        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy user theo email: {ex.Message}");
                return null;
            }
        }

        // Đếm số lượng Admins
        public async Task<int> CountAdmins()
        {
            try
            {
                return (int)await _users.CountDocumentsAsync(user => user.Role == "admin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đếm số lượng admin: {ex.Message}");
                return 0;
            }
        }
    }
}
