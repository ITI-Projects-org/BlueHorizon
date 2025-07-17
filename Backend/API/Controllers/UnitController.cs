using API.Models;
using API.UnitOfWorks;
using API.DTOs.UnitsDTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;

        public UnitController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> AddUnit([FromForm] AddUnitDTO unitDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var unit = _mapper.Map<Unit>(unitDto);
                unit.OwnerId = userId;

                // Handle contract document upload
                if (unitDto.ContractDocument != null)
                {
                    var fileName = $"unit_contract_{Guid.NewGuid()}{Path.GetExtension(unitDto.ContractDocument.FileName)}";
                    var filePath = Path.Combine("Uploads", "Contracts", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await unitDto.ContractDocument.CopyToAsync(stream);
                    }
                    unit.ContractPath = filePath;
                }

                await _unitOfWork.UnitRepository.AddAsync(unit);
                await _unitOfWork.SaveAsync();

                // Add amenities
                if (unitDto.AmenityIds != null && unitDto.AmenityIds.Any())
                {
                    foreach (var amenityId in unitDto.AmenityIds)
                    {
                        var unitAmenity = new UnitAmenity
                        {
                            UnitId = unit.Id,
                            AmenityId = amenityId
                        };
                        await _unitOfWork.UnitAmenityRepository.AddAsync(unitAmenity);
                    }
                    await _unitOfWork.SaveAsync();
                }

                return CreatedAtAction(nameof(AddUnit), new { id = unit.Id }, unit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
