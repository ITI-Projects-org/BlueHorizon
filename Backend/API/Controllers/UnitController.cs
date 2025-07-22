<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Village_System.DTOs.UnitDTO;
using Village_System.Mappers;
using Village_System.Models;
using Village_System.UnitOfWorks;

namespace Village_System.Controllers
=======
﻿using API.Models;
using API.UnitOfWorks;
using API.DTOs.UnitsDTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers
>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
        private readonly IMapper map;
        private readonly IUnitOfWork unitofwork;

        public UnitController(IMapper map ,IUnitOfWork unitofwork)
        {
            this.map = map;
            this.unitofwork = unitofwork;
        }

        #region Unit Details

=======
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

                unit.VerificationStatus = VerificationStatus.Pending;
                unit.CreationDate = DateTime.Now;


                // Handle contract document upload
                //if (unitDto.ContractDocument != null)
                //{
                //    var fileName = $"unit_contract_user:{userId}{Guid.NewGuid()}{Path.GetExtension(unitDto.ContractDocument.FileName)}";
                //    var filePath = Path.Combine("Uploads", "Contracts", fileName);
                //    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await unitDto.ContractDocument.CopyToAsync(stream);
                //    }
                //    unit.ContractPath = filePath;
                //}

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

                unit.VerificationStatus = VerificationStatus.Pending;

                return CreatedAtAction(nameof(AddUnit), new { id = unit.Id }, unitDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        #region Real Add Unit 
        [HttpPost("VerifyUnit/{id:int}")]
        //[Authorize(Roles = "Owner")]

        public async Task<IActionResult> VerifyUnit( int id)
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
        #endregion

        #region Unit Details
>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
        // Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {
<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
            var unit = await unitofwork.UnitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound("Unit Not Found");
            }
            return Ok(map.Map<UnitDetailsDTO>(unit));
=======
            var unit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return Content("Unit Not Found");
            }
            return Ok(_mapper.Map<UnitDetailsDTO>(unit));
>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
        }
        #endregion

        #region  Update Unit

        [HttpPut("{id}")]
<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
        public async Task<IActionResult> UpdateUnit([FromBody]UnitDetailsDTO unitDto , int id)
=======
        public async Task<IActionResult> UpdateUnit([FromBody] UnitDetailsDTO unitDto, int id)
>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
        {

            if (unitDto == null || !ModelState.IsValid)
            {
                return BadRequest("Unit data is null");
            }
<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
            var existingUnit = await unitofwork.UnitRepository.GetByIdAsync(id);
=======
            var existingUnit = await _unitOfWork.UnitRepository.GetByIdAsync(id);
>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
            if (existingUnit == null)
            {
                return NotFound("Unit Not Found");
            }
<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
            var unit = map.Map<Unit>(unitDto);
            try
            {
                unitofwork.UnitRepository.UpdateByIdAsync(id, unit);
                await unitofwork.SaveAsync();
=======
            var unit = _mapper.Map<Unit>(unitDto);
            try
            {
                _unitOfWork.UnitRepository.UpdateByIdAsync(id, unit);
                await _unitOfWork.SaveAsync();
>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error updating unit: {ex.Message}");
            }
            return Ok("Unit Updated Successfully");
        }

        #endregion
<<<<<<< HEAD:Village_System/Controllers/UnitController.cs
=======

>>>>>>> af77d17b32456f3cc70f92311a0f5263eecd7dee:Backend/API/Controllers/UnitController.cs
    }
}
