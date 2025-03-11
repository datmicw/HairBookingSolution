using HairBooking__API.Models;
using HairBooking__API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HairBooking__API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // ✅ Lấy danh sách User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        // ✅ Lấy User theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound("User not found!");
            return Ok(user);
        }

        // ✅ Thêm User mới
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
        {
            await _userService.CreateUser(newUser);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        // ✅ Cập nhật User
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] User updatedUser)
        {
            if (!await _userService.UpdateUser(id, updatedUser)) return NotFound("User not found!");
            return Ok(updatedUser);
        }

        // ✅ Xóa User
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (!await _userService.DeleteUser(id)) return NotFound("User not found!");
            return NoContent();
        }
    }
}
