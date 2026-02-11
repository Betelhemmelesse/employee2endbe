namespace EmployeeHierarchy.Api.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // Engineering, HR, etc.
        
        // Relationship: One department has many positions
        public ICollection<Position> Positions { get; set; } = new List<Position>();
    }
}