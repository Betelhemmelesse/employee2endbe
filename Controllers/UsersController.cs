using EmployeeHierarchy.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHierarchy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 👈 CHANGE THIS: Now any logged-in user can enter the "building"
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // 1. Profile - Anyone logged in (Admin or User) can see this
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var userId = Guid.Parse(userIdClaim);
                var user = await _userService.GetByIdAsync(userId);

                if (user == null) return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. Get All Users - ONLY ADMINS
        [HttpGet]
        [Authorize(Roles = "Admin")] // 🔒 Keep the Admin lock HERE
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 3. Delete User - ONLY ADMINS
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // 🔒 Keep the Admin lock HERE
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}