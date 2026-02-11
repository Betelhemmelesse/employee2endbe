namespace EmployeeHierarchy.Api.Dtos
{
    // Used for creating
    public class CreatePositionDto 
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }

    // Used for updating
    public class UpdatePositionDto : CreatePositionDto { }

    // Used for reading (The Tree Structure)
    public class PositionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
        public List<PositionDto> Children { get; set; } = new List<PositionDto>();
    }
}