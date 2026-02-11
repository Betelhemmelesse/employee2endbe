using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Dtos;
using EmployeeHierarchy.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Services
{
    public class DepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        // GET BY ID
        public async Task<DepartmentDto?> GetByIdAsync(Guid id)
        {
            var dept = await _context.Departments
                .Include(d => d.Positions)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dept == null) return null;

            return new DepartmentDto
            {
                Id = dept.Id,
                Name = dept.Name,
                PositionCount = dept.Positions.Count
            };
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var dept = await _context.Departments
                .Include(d => d.Positions)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dept == null) return false;

            // SAFETY CHECK: Prevent deletion if positions exist
            if (dept.Positions.Any())
            {
                throw new Exception("Cannot delete department because it contains active positions. Move the positions first.");
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper for your existing POST method
        public async Task<Department> CreateAsync(CreateDepartmentDto dto)
        {
            var dept = new Department { Id = Guid.NewGuid(), Name = dto.Name };
            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();
            return dept;
        }

        // Helper for your existing GET ALL method
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
{
    return await _context.Departments
        .Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            PositionCount = d.Positions.Count
        })
        .ToListAsync();
}
    }
}