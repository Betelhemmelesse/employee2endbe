namespace EmployeeHierarchy.Api.Dtos
{
    // Used for creating
    public class CreatePositionDto 
    {
        public string Name { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; } // NEW: Department association
        public Guid? ParentId { get; set; }
    }

    // Used for updating
    public class UpdatePositionDto : CreatePositionDto { }

    // Used for reading (The Tree Structure)
    public class PositionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; } // 👈 Return this so the UI knows which dept is selected
        public string? DepartmentName { get; set; } // 👈 Helpful for the UI labels
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
        public List<PositionDto> Children { get; set; } = new List<PositionDto>();
    }
}