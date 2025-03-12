using HairBooking__API.Models;
using HairBooking__API.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HairBooking__API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;

        private readonly UserService _userService;
        private readonly EncryptionHelper _encryptionHelper;

        public UserController(UserService userService, AuthService authService, EncryptionHelper encryptionHelper)
        {
            _userService = userService;
            _authService = authService;
            _encryptionHelper = encryptionHelper;
        }
        // Tạo User mới
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] User newUser)
        {
            var existingUser = await _userService.GetUserByEmail(newUser.Email);
            if (existingUser != null) return Conflict("User already exists!");
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            // mã hóa phone và address
            newUser.Phone = _encryptionHelper.Encrypt(newUser.Phone); 
            newUser.Address = _encryptionHelper.Encrypt(newUser.Address);

            await _userService.CreateUser(newUser);
            var token = _authService.GenerateJwtToken(newUser.Id, newUser.Email);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, new { user = newUser, token });
        }
        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByEmail(request.Email);
            if (user == null) return Unauthorized("Invalid credentials!"); // Kiểm tra uer có tòn tại không
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid credentials!");
            }
            var token = _authService.GenerateJwtToken(user.Id, user.Email);
            return Ok(new { token });
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        // lấy User theo ID
        [HttpGet("getbyId/{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound("User not found!");
            // giải mã phone và address
            user.Phone = _encryptionHelper.Decrypt(user.Phone);
            user.Address = _encryptionHelper.Decrypt(user.Address);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("updateUser/{id}")]
        public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var userId = User.FindFirst("sub")?.Value; // Lấy userId từ token
            if (userId == null) return Unauthorized("Invalid token");

            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound("User not found!");

            // Kiểm tra nếu user không phải admin và không phải chính user đó -> Cấm cập nhật
            var userRole = User.FindFirst("role")?.Value;
            if (userRole != "admin" && user.Id != userId)
            {
                return Forbid("You can only update your own profile!");
            }

            if (!await _userService.UpdateUser(id, updatedUser)) return NotFound("User not found!");
            return Ok(updatedUser);
        }

        // xóa User (Chỉ cho phép Admin xóa)
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (!await _userService.DeleteUser(id)) return NotFound("User not found!");
            return NoContent();
        }
    }
}
