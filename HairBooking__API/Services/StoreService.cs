using HairBooking__API.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HairBooking__API.Services
{
    public class StoreService
    {
        private readonly IMongoCollection<Store> _store; // Chỉnh sửa đúng tên biến collection

        public StoreService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDBSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDBSettings:DatabaseName"]);
            _store = database.GetCollection<Store>("Stores"); 
        }

        // Lấy danh sách Stores
        public async Task<List<Store>> GetAllStores() => await _store.Find(store => true).ToListAsync();

        // Thêm Store mới
        public async Task CreateStore(Store store) => await _store.InsertOneAsync(store);

        // Cập nhật Store theo ID
        public async Task<bool> UpdateStore(string id, Store updatedStore)
        {
            var result = await _store.ReplaceOneAsync(store => store.Id == id, updatedStore);
            return result.ModifiedCount > 0;
        }

        // Xóa Store theo ID
        public async Task<bool> DeleteStore(string id)
        {
            var result = await _store.DeleteOneAsync(store => store.Id == id);
            return result.DeletedCount > 0;
        }

        // Lấy Store theo ID
        public async Task<Store?> GetStoreById(string id)
        {
            return await _store.Find(store => store.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<Store>> GetStoresByUserId(string userId)
        {
            return await _store.Find(store => store.OwnerId == userId).ToListAsync();
        }
    }
}
