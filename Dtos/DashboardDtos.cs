namespace EmployeeHierarchy.Api.Dtos
{
    public class DashboardStatsDto
    {
        public int TotalDepartments { get; set; }
        public List<DepartmentDistributionDto> Distribution { get; set; } = new();
    }

    public class DepartmentDistributionDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public double Percentage { get; set; } // For the progress bar
    }
}