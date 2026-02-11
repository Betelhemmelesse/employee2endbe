using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var departments = await _context.Departments
                .Include(d => d.Positions)
                .ToListAsync();

            var totalEmployees = await _context.Positions.CountAsync(p => !string.IsNullOrEmpty(p.EmployeeName));

            return new DashboardStatsDto
            {
                TotalDepartments = departments.Count,
                Distribution = departments.Select(d => new DepartmentDistributionDto
                {
                    DepartmentName = d.Name,
                    EmployeeCount = d.Positions.Count(p => !string.IsNullOrEmpty(p.EmployeeName)),
                    Percentage = totalEmployees == 0 ? 0 : 
                        (double)d.Positions.Count(p => !string.IsNullOrEmpty(p.EmployeeName)) / totalEmployees * 100
                }).ToList()
            };
        }
    }
}