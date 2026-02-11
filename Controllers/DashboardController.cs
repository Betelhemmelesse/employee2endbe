using EmployeeHierarchy.Api.Services; // Fixes DashboardService error
using Microsoft.AspNetCore.Mvc;        // Fixes ControllerBase and [Route] error
using System.Threading.Tasks;          // Fixes Task error

namespace EmployeeHierarchy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;
        private readonly ActivityService _activityService;

        public DashboardController(DashboardService service, ActivityService activityService) // 👈 Update Constructor
        {
            _service = service;
            _activityService = activityService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _service.GetDashboardStatsAsync();
                 var activities = await _activityService.GetRecentActivitiesAsync(); 
                 return Ok(new 
                { 
                    Stats = stats, 
                    RecentActivity = activities 
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}