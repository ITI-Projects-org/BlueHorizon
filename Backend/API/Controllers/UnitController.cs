using API.Models;
using API.UnitOfWorks;
using API.DTOs.UnitDTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using API.Repositories.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {

        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UnitController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }



        [HttpGet("All")]
        public async Task<ActionResult> GetAll()
        {

            //var units = await _unitOfWork.UnitRepository.GetAllAsync();
            var units = await _unitOfWork.UnitRepository.GetAllValidUnits();
            //var units = await _unitOfWork.UnitRepository.GetAllFiltered();
            List<UnitDTO>? unitsdto = _mapper.Map<List<UnitDTO>>(units);

            foreach (var unitdto in unitsdto)
            {
                unitdto.UnitId = unitdto.Id;
                unitdto.imageURL = await _unitOfWork.UnitRepository.GetSingleImagePathByUnitId(unitdto.Id);
            }

            return Ok(unitsdto);

        }

        [HttpDelete]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                var existingUnit = await _unitOfWork.UnitRepository.GetUnitWithDetailsAsync(id);
                if (existingUnit == null)
                {
                    return NotFound("This Unit Does not exist");
                }
                //var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if (existingUnit.OwnerId != currentUserId)
                //{
                //    return Forbid("You Cannot Delete This Unit.");
                //}
                _unitOfWork.UnitRepository.DeleteByIdAsync(id);

                return Ok(new { Message = "Unit Deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [Authorize(Roles = "Owner")]
        [HttpPost("AddUnit")]
        public async Task<IActionResult> AddUnit([FromForm] AddUnitDTO unitDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var unit = _mapper.Map<Unit>(unitDto);
                unit.OwnerId = userId;

                unit.VerificationStatus = VerificationStatus.Pending;
                unit.CreationDate = DateTime.Now;


                //Handle contract document upload
                //if (unitDto.ContractDocument != null)
                //{
                //    var fileName = $"unit_contract_user:{Guid.NewGuid()}{Path.GetExtension(unitDto.ContractDocument.FileName)}";
                //    var filePath = Path.Combine("Uploads", "Contracts", fileName);
                //    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await unitDto.ContractDocument.CopyToAsync(stream);
                //    }
                //    unit.ContractPath = filePath;
                //}
                if (unitDto.ContractDocument != null)
                {
                    var res = await _photoService.AddPhotoAsync(unitDto.ContractDocument);

                    unit.ContractPath = res.Url.ToString();
                }

                await _unitOfWork.UnitRepository.AddAsync(unit);
                await _unitOfWork.SaveAsync();
                if (unitDto.UnitImages != null && unitDto.UnitImages.Any())
                {
                    foreach (var image in unitDto.UnitImages)
                    {
                        var res = await _photoService.AddPhotoAsync(image);
                        //res.Url
                        UnitImages unitImage = new UnitImages()
                        {
                            UnitID = unit.Id,
                            ImageURL = res.Url.ToString()
                        };
                        await _unitOfWork.UnitImagesRepository.AddAsync(unitImage);
                    }
                }



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

                unit.VerificationStatus = VerificationStatus.Pending;
                await _unitOfWork.SaveAsync();
                return CreatedAtAction(nameof(AddUnit), new { id = unit.Id }, unitDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("VerifyUnit/{id:int}")]
        public async Task<IActionResult> VerifyUnit(int id)
        {
            var unit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            _mapper.Map<Unit>(unit);
            unit.Id = id;
            if (unit != null)
            {
                unit.VerificationStatus = VerificationStatus.Verified;
                _unitOfWork.UnitRepository.UpdateByIdAsync(unit.Id, unit);
                _unitOfWork.SaveAsync();
                return CreatedAtAction(nameof(VerifyUnit), new { id = unit.Id }, _mapper.Map<UnitDetailsDTO>(unit));
            }
            else
            {
                return BadRequest();
            }
        }

        // Get By Id
        [HttpGet("GetUnitById/{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {

            var unit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return Content("Unit Not Found");
            }
            return Ok(_mapper.Map<UnitDetailsDTO>(unit));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit([FromBody] UnitDetailsDTO unitDto, int id)
        {

            if (unitDto == null || !ModelState.IsValid)
            {
                return BadRequest("Unit data is null");
            }

            var existingUnit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            if (existingUnit == null)
            {
                return NotFound("Unit Not Found");
            }

            var unit = _mapper.Map<Unit>(unitDto);
            try
            {
                _unitOfWork.UnitRepository.UpdateByIdAsync(id, unit);
                await _unitOfWork.SaveAsync();
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error updating unit: {ex.Message}");
            }
            return Ok("Unit Updated Successfully");
        }

        [HttpPut("DeleteUnit/{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            _unitOfWork.UnitRepository.DeleteByIdAsync(id);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
