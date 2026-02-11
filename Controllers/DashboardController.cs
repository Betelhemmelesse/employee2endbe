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

        public DashboardController(DashboardService service)
        {
            _service = service;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _service.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}