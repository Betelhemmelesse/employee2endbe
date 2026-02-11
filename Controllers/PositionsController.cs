using EmployeeHierarchy.Api.Dtos;
using EmployeeHierarchy.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHierarchy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 PROTECTS ALL ENDPOINTS BELOW
    public class PositionsController : ControllerBase
    {
        private readonly PositionService _service;

        public PositionsController(PositionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            return Ok(await _service.GetTreeAsync());
        }

        

        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionDto dto)
        {
            try {
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    try 
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) 
        {
            return NotFound(new { message = $"Position with ID {id} not found." });
        }
        return Ok(result);
    } 
    catch (Exception ex) 
    { 
        return BadRequest(ex.Message); 
    }
}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdatePositionDto dto)
        {
            try {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try {
                await _service.DeleteAsync(id);
                return NoContent();
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}