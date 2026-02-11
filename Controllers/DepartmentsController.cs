using EmployeeHierarchy.Api.Dtos;      // Fixes the "CreateDepartmentDto" error
using EmployeeHierarchy.Api.Entities;
using EmployeeHierarchy.Api.Services;  // Needed to use DepartmentService
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentService _service;

        public DepartmentsController(DepartmentService service)
        {
            _service = service;
        }

        // 1. GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            // We use the service to get a clean list
            var departments = await _service.GetAllAsync();
            return Ok(departments);
        }

        // 2. GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dept = await _service.GetByIdAsync(id);
            if (dept == null) return NotFound();
            return Ok(dept);
        }

        // 3. POST (CREATE)
        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 4. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success) return NotFound();
                return NoContent(); // Success 204
            }
            catch (System.Exception ex)
            {
                // This catches the error if the department still has positions
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}