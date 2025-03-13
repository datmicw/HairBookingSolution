using System.Security.Claims;
using HairBooking__API.Models;
using HairBooking__API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace HairBooking__API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreService _storeService;
        private readonly AuthService _authService;
        public StoreController(StoreService storeService, AuthService authService)
        {
            _storeService = storeService;
            _authService = authService;
        }
        [HttpPost("register-store")]
        public async Task<ActionResult<Store>> RegisterStore([FromBody] Store newStore)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized("Unauthorized!");

            newStore.OwnerId = userId;   

            Console.WriteLine($"📢 Registering Store: {newStore.StoreName}");

            if (string.IsNullOrEmpty(newStore.StoreName) || string.IsNullOrEmpty(newStore.StoreAddress))
                return BadRequest("Name and Address are required!");

            if (!string.IsNullOrEmpty(newStore.Id) && ObjectId.TryParse(newStore.Id, out _))
            {
                var existingStore = await _storeService.GetStoreById(newStore.Id);
                if (existingStore != null) return Conflict("Store already exists!");
            }

            Console.WriteLine($"✅ Creating Store: {newStore.StoreName} - {newStore.StoreAddress}");
            await _storeService.CreateStore(newStore);
            return Ok(newStore);
        }
        [HttpGet("get-store/{storeId}")]
        public async Task<ActionResult<Store>> GetStore(string storeId)
        {
            var store = await _storeService.GetStoreById(storeId);
            if (store == null) return NotFound("Store not found!");
            return Ok(store);
        }
        [HttpGet("get-stores-by-user/{userId}")]
        public async Task<ActionResult<List<Store>>> GetStores(string userId)
        {
            var stores = await _storeService.GetStoresByUserId(userId);
            if (stores == null || stores.Count == 0) return NotFound("Stores not found!");
            return Ok(stores);
        }

    }
}