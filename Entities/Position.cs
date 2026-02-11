using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeHierarchy.Api.Entities
{
    public class Position
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "CTO"

        public string Description { get; set; } = string.Empty;

        // Self-Referencing Logic
        public Guid? ParentId { get; set; } // Null if it's the top boss (Root)
        
        [JsonIgnore] // Prevent cycles in JSON
        public Position? Parent { get; set; }
        
        public ICollection<Position> Children { get; set; } = new List<Position>();
    }
}