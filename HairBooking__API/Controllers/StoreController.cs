using System.Security.Claims;
using HairBooking__API.Models;
using HairBooking__API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairBooking__API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreService _storeService;
        private readonly AuthService _authService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(StoreService storeService, AuthService authService, ILogger<StoreController> logger)
        {
            _storeService = storeService;
            _authService = authService;
            _logger = logger;
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPost("register-store")]
        public async Task<ActionResult<StoreResponse>> RegisterStore([FromBody] Store newStore)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized("Unauthorized!");

                newStore.OwnerId = userId;
                _logger.LogInformation("📢 Registering Store: {Name}", newStore.StoreName);

                if (string.IsNullOrEmpty(newStore.StoreName) || string.IsNullOrEmpty(newStore.StoreAddress))
                    return BadRequest("Name and Address are required!");

                var existingStore = await _storeService.GetStoreByName(newStore.StoreName);
                if (existingStore != null) return Conflict("Store with this name already exists!");

                newStore.CreatedAt = DateTime.UtcNow;
                newStore.UpdatedAt = DateTime.UtcNow;
                newStore.StoreSlug = newStore.StoreName.ToLower().Replace(" ", "-");
                await _storeService.CreateStore(newStore);

                var response = new StoreResponse
                {
                    Id = newStore.Id,
                    StoreName = newStore.StoreName,
                    StoreAddress = newStore.StoreAddress,
                    StorePhone = newStore.StorePhone ?? string.Empty,
                    StoreEmail = newStore.StoreEmail ?? string.Empty,
                    StoreBio = newStore.StoreBio,
                    CreatedAt = newStore.CreatedAt,
                    UpdatedAt = newStore.UpdatedAt,
                    StoreSlug = newStore.StoreSlug
                };

                _logger.LogInformation("✅ Store registered: {Name}", newStore.StoreName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error registering store: {Name}", newStore.StoreName);
                return StatusCode(500, "An error occurred while registering the store.");
            }
        }

        [HttpGet("get-store/{storeId}")]
        public async Task<ActionResult<StoreResponse>> GetStore(string storeId)
        {
            try
            {
                var store = await _storeService.GetStoreById(storeId);
                if (store == null || store.IsDeleted) return NotFound("Store not found!");

                var response = new StoreResponse
                {
                    Id = store.Id,
                    StoreName = store.StoreName ?? string.Empty,
                    StoreAddress = store.StoreAddress   ?? string.Empty,
                    StorePhone = store.StorePhone ?? string.Empty,
                    StoreEmail = store.StoreEmail ?? string.Empty,
                    StoreBio = store.StoreBio,
                    CreatedAt = store.CreatedAt,
                    UpdatedAt = store.UpdatedAt,
                    StoreSlug = store.StoreSlug
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving store: {StoreId}", storeId);
                return StatusCode(500, "An error occurred while retrieving the store.");
            }
        }

        [HttpGet("get-stores-by-user/{userId}")]
        public async Task<ActionResult<List<StoreResponse>>> GetStores(string userId)
        {
            try
            {
                var stores = await _storeService.GetStoresByUserId(userId);
                if (stores == null || stores.Count == 0) return NotFound("Stores not found!");

                var response = stores.Where(s => !s.IsDeleted).Select(store => new StoreResponse
                {
                    Id = store.Id,
                    StoreName = store.StoreName ?? string.Empty,
                    StoreAddress = store.StoreAddress   ?? string.Empty,
                    StorePhone = store.StorePhone ?? string.Empty,
                    StoreEmail = store.StoreEmail ?? string.Empty,
                    StoreBio = store.StoreBio,
                    CreatedAt = store.CreatedAt,
                    UpdatedAt = store.UpdatedAt,
                    StoreSlug = store.StoreSlug
                }).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving stores for user: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving stores.");
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPut("update-store/{storeId}")]
        public async Task<ActionResult<StoreResponse>> UpdateStore(string storeId, [FromBody] Store updatedStore)
        {
            try
            {
                var store = await _storeService.GetStoreById(storeId);
                if (store == null || store.IsDeleted) return NotFound("Store not found!");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (store.OwnerId != userId && !User.IsInRole("Admin")) return Forbid("You do not own this store!");

                store.StoreName = updatedStore.StoreName;
                store.StoreAddress = updatedStore.StoreAddress;
                store.StorePhone = updatedStore.StorePhone;
                store.StoreEmail = updatedStore.StoreEmail;
                store.StoreBio = updatedStore.StoreBio;
                store.UpdatedAt = DateTime.UtcNow;

                await _storeService.UpdateStore(storeId, store);
                var response = new StoreResponse { /* mapping */ };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating store: {StoreId}", storeId);
                return StatusCode(500, "An error occurred while updating the store.");
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpDelete("delete-store/{storeId}")]
        public async Task<ActionResult> DeleteStore(string storeId)
        {
            try
            {
                var store = await _storeService.GetStoreById(storeId);
                if (store == null) return NotFound("Store not found!");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (store.OwnerId != userId && !User.IsInRole("Admin")) return Forbid("You do not own this store!");

                await _storeService.DeleteStore(storeId);
                return Ok("Store deleted!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting store: {StoreId}", storeId);
                return StatusCode(500, "An error occurred while deleting the store.");
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPut("delete-store/{storeId}/soft")]
        public async Task<ActionResult> SoftDeleteStore(string storeId)
        {
            try
            {
                var store = await _storeService.GetStoreById(storeId);
                if (store == null) return NotFound("Store not found!");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (store.OwnerId != userId && !User.IsInRole("Admin")) return Forbid("You do not own this store!");

                store.IsDeleted = true;
                store.UpdatedAt = DateTime.UtcNow;
                await _storeService.UpdateStore(storeId, store);
                return Ok("Store soft deleted!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error soft deleting store: {StoreId}", storeId);
                return StatusCode(500, "An error occurred while soft deleting the store.");
            }
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPut("restore-store/{storeId}/restore")]
        public async Task<ActionResult> RestoreStore(string storeId)
        {
            try
            {
                var store = await _storeService.GetStoreById(storeId);
                if (store == null) return NotFound("Store not found!");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (store.OwnerId != userId && !User.IsInRole("Admin")) return Forbid("You do not own this store!");

                store.IsDeleted = false;
                store.UpdatedAt = DateTime.UtcNow;
                await _storeService.UpdateStore(storeId, store);
                return Ok("Store restored!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error restoring store: {StoreId}", storeId);
                return StatusCode(500, "An error occurred while restoring the store.");
            }
        }
    }

    public class StoreResponse
    {
        public string Id { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public string StoreEmail { get; set; }
        public string StoreBio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string StoreSlug { get; set; }
    }
}