using HairBooking__API.Models;
using HairBooking__API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairBooking__API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, AuthService authService, ILogger<UserController> logger)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register-user")]
        public async Task<ActionResult<UserResponse>> RegisterUser([FromBody] RegisterRequest newUser)
        {
            try
            {
                _logger.LogInformation("📢 Registering User: {Email}", newUser.Email);
                if (string.IsNullOrEmpty(newUser.Email) || string.IsNullOrEmpty(newUser.Password))
                    return BadRequest("Email and Password are required!");

                var existingUser = await _userService.GetUserByEmail(newUser.Email);
                if (existingUser != null) return Conflict("User already exists!");

                var user = new User
                {
                    Name = newUser.Name,
                    Email = newUser.Email,
                    Phone = newUser.Phone,
                    Address = newUser.Address,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SlugCustomer = newUser.Email.ToLower().Replace("@", "-").Replace(".", "-") 
                };

                await _userService.CreateUser(user);
                var token = _authService.GenerateJwtToken(user.Id, user.Email, UserRole.User.ToString());

                var response = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    StoreId = user.StoreId,
                    SlugCustomer = user.SlugCustomer
                };

                _logger.LogInformation("✅ User registered: {Email}", user.Email);
                return Ok(new { token, user = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error registering user: {Email}", newUser.Email);
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.EmailCustomer) || string.IsNullOrEmpty(request.Password))
                    return BadRequest("Email and Password are required!");

                var user = await _userService.GetUserByEmail(request.EmailCustomer);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return Unauthorized("Invalid credentials!");

                var token = _authService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
                return Ok(new { token, role = user.Role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error logging in user: {Email}", request.EmailCustomer);
                return StatusCode(500, "An error occurred while logging in.");
            }
        }

        [HttpGet("get-user/{userId}")]
        public async Task<ActionResult<UserResponse>> GetUser(string userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null) return NotFound("User not found!");

                var response = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    StoreId = user.StoreId,
                    SlugCustomer = user.SlugCustomer
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving user: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                var response = users.Select(user => new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    StoreId = user.StoreId,
                    SlugCustomer = user.SlugCustomer
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving all users");
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string EmailCustomer { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? StoreId { get; set; }
        public string SlugCustomer { get; set; }
    }
}