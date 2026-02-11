using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Dtos;
using EmployeeHierarchy.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Services
{
    public class PositionService
    {
        private readonly AppDbContext _context;

        public PositionService(AppDbContext context)
        {
            _context = context;
        }

        // GET: Build Tree
        public async Task<List<PositionDto>> GetTreeAsync()
        {
            var positions = await _context.Positions.ToListAsync();

            // Map Entity to DTO
            var dtos = positions.Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
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
                Description = dto.Description,
                ParentId = dto.ParentId
            };

            _context.Positions.Add(pos);
            await _context.SaveChangesAsync();
            return pos;
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, UpdatePositionDto dto)
        {
            var pos = await _context.Positions.FindAsync(id);
            if (pos == null) throw new KeyNotFoundException("Position not found");

            // Prevent circular reference (Can't be your own parent)
            if (dto.ParentId == id) throw new Exception("Position cannot be its own parent");

            pos.Name = dto.Name;
            pos.Description = dto.Description;
            pos.ParentId = dto.ParentId;

            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var pos = await _context.Positions.Include(p => p.Children).FirstOrDefaultAsync(p => p.Id == id);
            if (pos == null) throw new KeyNotFoundException("Position not found");

            if (pos.Children.Any())
                throw new Exception("Cannot delete position with subordinates. Reassign them first.");

            _context.Positions.Remove(pos);
            await _context.SaveChangesAsync();
        }
    }
}