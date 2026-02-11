using EmployeeHierarchy.Api.Dtos;
using EmployeeHierarchy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHierarchy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try {
                await _authService.RegisterAsync(dto);
                return Ok(new { message = "User registered successfully" });
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try {
                var token = await _authService.LoginAsync(dto);
                return Ok(new AuthResponseDto(token, dto.Username));
            } catch (Exception ex) { return Unauthorized(ex.Message); }
        }
    }
}