using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Village_System.DTOs.UnitDTO;
using Village_System.Mappers;
using Village_System.Models;
using Village_System.UnitOfWorks;

namespace Village_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IMapper map;
        private readonly IUnitOfWork unitofwork;

        public UnitController(IMapper map ,IUnitOfWork unitofwork)
        {
            this.map = map;
            this.unitofwork = unitofwork;
        }

        #region Unit Details

        // Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {
            var unit = await unitofwork.UnitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound("Unit Not Found");
            }
            return Ok(map.Map<UnitDetailsDTO>(unit));
        }
        #endregion

        #region  Update Unit

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit([FromBody]UnitDetailsDTO unitDto , int id)
        {

            if (unitDto == null || !ModelState.IsValid)
            {
                return BadRequest("Unit data is null");
            }
            var existingUnit = await unitofwork.UnitRepository.GetByIdAsync(id);
            if (existingUnit == null)
            {
                return NotFound("Unit Not Found");
            }
            var unit = map.Map<Unit>(unitDto);
            try
            {
                unitofwork.UnitRepository.UpdateByIdAsync(id, unit);
                await unitofwork.SaveAsync();
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error updating unit: {ex.Message}");
            }
            return Ok("Unit Updated Successfully");
        }

        #endregion
    }
}
