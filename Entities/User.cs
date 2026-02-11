namespace EmployeeHierarchy.Api.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Store hashed password, not plain text!
        public string Role { get; set; } = "User"; // Simple role for authorization
    }
}