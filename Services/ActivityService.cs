using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Services
{
    public class ActivityService
    {
        private readonly AppDbContext _context;

        public ActivityService(AppDbContext context) => _context = context;

        // Record a new action
        public async Task LogAsync(string title, string message, string type)
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        // Get the latest 5-10 activities for the dashboard
        public async Task<List<ActivityLog>> GetRecentActivitiesAsync()
        {
            return await _context.ActivityLogs
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();
        }
    }
}