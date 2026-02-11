using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeHierarchy.Api.Entities
{
   public class Position
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g., "Chief Executive Officer"

    // ADD THIS
    public string EmployeeName { get; set; } = string.Empty; // e.g., "John Anderson"

    public string Description { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }
    
    [JsonIgnore]
    public Position? Parent { get; set; }

    // dep't
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    public ICollection<Position> Children { get; set; } = new List<Position>();
}
}