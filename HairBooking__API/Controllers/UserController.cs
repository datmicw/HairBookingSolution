using HairBooking__API.Models;
using HairBooking__API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HairBooking__API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public UserController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] User newUser)
        {
            Console.WriteLine($"📢 Registering User: {newUser.Email}");

            if (string.IsNullOrEmpty(newUser.Email) || string.IsNullOrEmpty(newUser.Password))
                return BadRequest("Email and Password are required!");

            var existingUser = await _userService.GetUserByEmail(newUser.Email);
            if (existingUser != null) return Conflict("User already exists!");

            // Hash password before saving
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            newUser.Role = "user";
            newUser.CreatedAt = DateTime.UtcNow;

            Console.WriteLine($"✅ Creating User: {newUser.Email} - {newUser.Role}");

            await _userService.CreateUser(newUser);
            var token = _authService.GenerateJwtToken(newUser.Id, newUser.Email, newUser.Role);

            return Ok(new { token, user = newUser });
        }

        [HttpPost("user-login")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string> request)
        {
            if (request == null || !request.ContainsKey("email_customer") || !request.ContainsKey("password"))
                return BadRequest("Email and Password are required!");

            string email = request["email_customer"];
            string password = request["password"];

            var user = await _userService.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return Unauthorized("Invalid credentials!");

            var token = _authService.GenerateJwtToken(user.Id, user.Email, user.Role);
            return Ok(new { token, role = user.Role });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

    }
}
