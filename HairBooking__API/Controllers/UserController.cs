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

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            newUser.Role = "user";

            Console.WriteLine($"✅ Creating User: {newUser.Email} - {newUser.Role}");

            await _userService.CreateUser(newUser);
            var token = _authService.GenerateJwtToken(newUser.Id, newUser.Email, newUser.Role);

            return Ok(new { token, user = newUser });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByEmail(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid credentials!");

            var token = _authService.GenerateJwtToken(user.Id, user.Email, user.Role);
            return Ok(new { token, user.Role });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("debugClaims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new { claims });
        }
    }
}
