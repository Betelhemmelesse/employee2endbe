namespace EmployeeHierarchy.Api.Dtos
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int PositionCount { get; set; } // Shows how many positions are in this dept
    }

    public class CreateDepartmentDto
    {
        public required string Name { get; set; }
    }
}