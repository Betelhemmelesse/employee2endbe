using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Dtos;
using EmployeeHierarchy.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeHierarchy.Api.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

public async Task<User> RegisterAsync(RegisterDto dto)
{
    // 1. Check if username already exists
    if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
        throw new Exception("Username already exists");

    // 2. NEW: Check if email already exists
    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        throw new Exception("Email is already in use");

    // 3. Determine Role
    bool isFirstUser = !await _context.Users.AnyAsync();
    string assignedRole = isFirstUser ? "Admin" : "User";

    // 4. Create the new user object
    var user = new User
    {
        Id = Guid.NewGuid(),
        Username = dto.Username,
        Email = dto.Email, // 👈 Now this works because DTO has Email
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        Role = assignedRole
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return user;
}

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}