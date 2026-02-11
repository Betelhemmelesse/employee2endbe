namespace EmployeeHierarchy.Api.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;       // e.g., "Sarah Chen"
        public string Message { get; set; } = string.Empty;     // e.g., "joined as CTO"
        public string Type { get; set; } = string.Empty;        // e.g., "Join", "Promotion", "Update", "Delete"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}