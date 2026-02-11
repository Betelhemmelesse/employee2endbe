using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Dtos;
using EmployeeHierarchy.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Services
{
    public class PositionService
    {
        private readonly AppDbContext _context;
        private readonly ActivityService _activityService; 
        public PositionService(AppDbContext context, ActivityService activityService) 
        {
            _context = context;
            _activityService = activityService;
        }

        // GET: Build Tree
        public async Task<List<PositionDto>> GetTreeAsync()
        {
            var positions = await _context.Positions.Include(p => p.Department).ToListAsync();

            // Map Entity to DTO
            var dtos = positions.Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                EmployeeName = p.EmployeeName,
                Description = p.Description,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department?.Name,
                ParentId = p.ParentId
            }).ToList();

            // Build Hierarchy in Memory
            var dict = dtos.ToDictionary(x => x.Id);
            var rootNodes = new List<PositionDto>();

            foreach (var dto in dtos)
            {
                if (dto.ParentId.HasValue && dict.ContainsKey(dto.ParentId.Value))
                {
                    var parent = dict[dto.ParentId.Value];
                    parent.Children.Add(dto);
                    dto.ParentName = parent.Name;
                }
                else
                {
                    rootNodes.Add(dto);
                }
            }
            return rootNodes;
        }

        // CREATE
        public async Task<Position> CreateAsync(CreatePositionDto dto)
        {
            // Rule: Only one root allowed? (Optional, usually true for Hierarchy)
            if (dto.ParentId == null)
            {
                if (await _context.Positions.AnyAsync(p => p.ParentId == null))
                    throw new Exception("A Root position (CEO) already exists.");
            }

            var pos = new Position
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                EmployeeName = dto.EmployeeName,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId,
                ParentId = dto.ParentId
            };

            _context.Positions.Add(pos);
            await _context.SaveChangesAsync();

            await _activityService.LogAsync(dto.EmployeeName, $"joined as {dto.Name}", "Join");
            return pos;
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, UpdatePositionDto dto)
        {
            var pos = await _context.Positions.FindAsync(id);
            if (pos == null) throw new KeyNotFoundException("Position not found");

            // Prevent circular reference (Can't be your own parent)
            string oldName = pos.Name;

            pos.Name = !string.IsNullOrWhiteSpace(dto.Name) ? dto.Name : pos.Name;
            pos.EmployeeName = !string.IsNullOrWhiteSpace(dto.EmployeeName) ? dto.EmployeeName : pos.EmployeeName;
            pos.Description = !string.IsNullOrWhiteSpace(dto.Description) ? dto.Description : pos.Description;
            pos.DepartmentId = dto.DepartmentId != Guid.Empty ? dto.DepartmentId : pos.DepartmentId;
            pos.ParentId = dto.ParentId;

            await _context.SaveChangesAsync();

            if (oldName != pos.Name)
                await _activityService.LogAsync(pos.EmployeeName, $"promoted to {pos.Name}", "Update");
            else
                await _activityService.LogAsync("Hierarchy", "structure updated", "Update");
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var pos = await _context.Positions.Include(p => p.Children).FirstOrDefaultAsync(p => p.Id == id);
            if (pos == null) throw new KeyNotFoundException("Position not found");

            if (pos.Children.Any())
                throw new Exception("Cannot delete position with subordinates. Reassign them first.");

            await _activityService.LogAsync(pos.Name, "position removed", "Delete");

            _context.Positions.Remove(pos);
            await _context.SaveChangesAsync();
        }

        public async Task<PositionDto?> GetByIdAsync(Guid id)
{
    // 1. Find the position in the database
    var position = await _context.Positions
        .FirstOrDefaultAsync(p => p.Id == id);

    if (position == null) return null;

    // 2. Map the Database Entity to a DTO (Data Transfer Object)
    // Adjust these fields based on your actual PositionDto class
    return new PositionDto
{
    Id = position.Id,
    Name = position.Name,
    EmployeeName = position.EmployeeName, // ADD THIS
    Description = position.Description,
    ParentId = position.ParentId,
};
}

// add activityservice to the constructor


// error kehone atfiw
// Get the tree hierarchy but filtered for one department
public async Task<List<PositionDto>> GetTreeByDepartmentAsync(Guid departmentId)
{
    // 1. Only get positions belonging to this department
    var positions = await _context.Positions
        .Where(p => p.DepartmentId == departmentId)
        .ToListAsync();

    // 2. Map to DTOs
    var dtos = positions.Select(p => new PositionDto
    {
        Id = p.Id,
        Name = p.Name,
        EmployeeName = p.EmployeeName,
        ParentId = p.ParentId
    }).ToList();

    // 3. Build the mini-tree (same logic as your GetTreeAsync)
    var dict = dtos.ToDictionary(x => x.Id);
    var rootNodes = new List<PositionDto>();

    foreach (var dto in dtos)
    {
        if (dto.ParentId.HasValue && dict.ContainsKey(dto.ParentId.Value))
        {
            dict[dto.ParentId.Value].Children.Add(dto);
        }
        else
        {
            rootNodes.Add(dto);
        }
    }
    return rootNodes;
}
    }
}