using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            // We fetch from your custom Users table
            return await _context.Users
                .Select(user => new UserDto
                {
                    Id = user.Id.ToString(), // Converts Guid to string for the DTO
                    Email = user.Email,
                    UserName = user.Username, // Ensure this matches the property name in your User entity
                    Role = user.Role
                })
                .ToListAsync();
        }

        // getbyid
      // Inside Services/UserService.cs

public async Task<UserDto?> GetByIdAsync(Guid id)
{
    var user = await _context.Users.FindAsync(id);
    
    if (user == null) return null;

    return new UserDto
    {
        Id = user.Id.ToString(),
        UserName = user.Username,
        Email = user.Email,
        Role = user.Role
    };
}

        // delete user 
        public async Task<bool> DeleteUserAsync(Guid id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        return false;
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return true;
}
    }
}