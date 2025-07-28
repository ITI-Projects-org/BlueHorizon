using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.DTOs.UnitDTO;
using API.Models;
using API.UnitOfWorks;
using API.Repositories.Interfaces;
using AutoMapper;
using System.IO;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UnitController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitDTO>>> GetAllUnits()
        {
            var units = await _unitOfWork.UnitRepository.GetAllAsync();
            var unitsDto = _mapper.Map<IEnumerable<UnitDTO>>(units);
            return Ok(unitsDto);
        }

        [HttpGet("MyUnits")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> GetMyUnits()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var units = await _unitOfWork.UnitRepository.GetUnitsByOwnerIdAsync(userId);
            List<UnitDTO>? unitsdto = _mapper.Map<List<UnitDTO>>(units);
            foreach (var unitdto in unitsdto)
            {
                unitdto.UnitId = unitdto.Id;
                unitdto.imageURL = await _unitOfWork.UnitRepository.GetSingleImagePathByUnitId(unitdto.Id);
            }
            return Ok(unitsdto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitDTO>> GetUnitById(int id)
        {
            var unit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            if (unit == null)
                return NotFound();

            var unitDto = _mapper.Map<UnitDTO>(unit);
            unitDto.UnitId = unitDto.Id;
            unitDto.imageURL = await _unitOfWork.UnitRepository.GetSingleImagePathByUnitId(unitDto.Id);
            return Ok(unitDto);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<UnitDTO>> CreateUnit([FromForm] AddUnitDTO unitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var unit = _mapper.Map<Unit>(unitDto);
            unit.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            unit.VerificationStatus = "Pending";

            if (unitDto.Images != null && unitDto.Images.Any())
            {
                var imageUrls = new List<string>();
                foreach (var image in unitDto.Images)
                {
                    var res = await _photoService.AddPhotoAsync(image);
                    if (res.Error != null)
                        return BadRequest(res.Error.Message);
                    imageUrls.Add(res.SecureUrl.AbsoluteUri);
                }
                unit.ImagesPaths = string.Join(",", imageUrls);
            }

            await _unitOfWork.UnitRepository.AddAsync(unit);
            await _unitOfWork.SaveAsync();

            var createdUnitDto = _mapper.Map<UnitDTO>(unit);
            createdUnitDto.UnitId = createdUnitDto.Id;
            return CreatedAtAction(nameof(GetUnitById), new { id = unit.Id }, createdUnitDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateUnit(int id, [FromForm] UpdateUnitDTO unitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUnit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            if (existingUnit == null)
                return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (existingUnit.OwnerId != currentUserId)
                return Forbid();

            _mapper.Map(unitDto, existingUnit);

            if (unitDto.Images != null && unitDto.Images.Any())
            {
                var imageUrls = new List<string>();
                foreach (var image in unitDto.Images)
                {
                    var res = await _photoService.AddPhotoAsync(image);
                    if (res.Error != null)
                        return BadRequest(res.Error.Message);
                    imageUrls.Add(res.SecureUrl.AbsoluteUri);
                }
                existingUnit.ImagesPaths = string.Join(",", imageUrls);
            }

            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("DeleteUnit/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var unit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            if (unit == null)
                return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (unit.OwnerId != currentUserId)
                return Forbid();

            await _unitOfWork.UnitRepository.DeleteByIdAsync(id);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<UnitDTO>>> GetFilteredUnits(
            [FromQuery] string? village = null,
            [FromQuery] int? unitType = null,
            [FromQuery] int? bedrooms = null,
            [FromQuery] int? bathrooms = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var units = await _unitOfWork.UnitRepository.GetAllAsync();
            var filteredUnits = units.AsQueryable();

            if (!string.IsNullOrEmpty(village))
                filteredUnits = filteredUnits.Where(u => u.VillageName == village);

            if (unitType.HasValue)
                filteredUnits = filteredUnits.Where(u => u.UnitType == unitType.Value);

            if (bedrooms.HasValue)
                filteredUnits = filteredUnits.Where(u => u.Bedrooms == bedrooms.Value);

            if (bathrooms.HasValue)
                filteredUnits = filteredUnits.Where(u => u.Bathrooms == bathrooms.Value);

            if (minPrice.HasValue)
                filteredUnits = filteredUnits.Where(u => u.BasePricePerNight >= minPrice.Value);

            if (maxPrice.HasValue)
                filteredUnits = filteredUnits.Where(u => u.BasePricePerNight <= maxPrice.Value);

            var unitsDto = _mapper.Map<IEnumerable<UnitDTO>>(filteredUnits);
            return Ok(unitsDto);
        }
    }
}
