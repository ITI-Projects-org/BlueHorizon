﻿using System.Security.Claims;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.VerificationDTO;
using API.Models;
using API.UnitOfWorks;

namespace API.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
    
    public class VerificationController : ControllerBase
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }

        public VerificationController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpPost("AddRequest")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> OwnerVerificationRequest([FromBody]OwnerWithUnitVerificationDTO ownerVerificationDTO){
            #region Verify Owner

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = User.FindFirstValue("userId");
            ownerVerificationDTO.OwnerId = userId;
            var owner = await _unit.OwnerRepository.GetByIdAsync(userId);
            if (owner == null)
                return BadRequest("Owner not found");

            if (owner.VerificationStatus != VerificationStatus.NotVerified && owner.VerificationStatus != VerificationStatus.Rejected)
                return BadRequest("Owner already verified or pending");

            //OwnerVerificationDocument? ownerVerificationDocument = _mapper.Map<OwnerVerificationDocument>(ownerVerificationDTO);
            //ownerVerificationDocument.OwnerId = userId;
            //_unit.OwnerVerificationDocumentRepository.AddAsync(ownerVerificationDocument);

            owner.VerificationStatus = VerificationStatus.Pending; 
            owner.VerificationDate = DateTime.Now;
            #endregion

            #region Table VerificationOwnerDocument
            OwnerVerificationDocument doc = _mapper.Map<OwnerVerificationDocument>(ownerVerificationDTO);
            _unit.OwnerVerificationDocumentRepository.AddAsync(doc);
            #endregion

            #region Verify Unit
            Unit unit = _mapper.Map<Unit>(ownerVerificationDTO);
            _unit.UnitRepository.AddAsync(unit);
            //unit.UnitAmenities = ownerVerificationDTO.UnitAmenities;
            #endregion
            await _unit.SaveAsync();
            return Ok();
        }
        [HttpGet("Requests")]
        [Authorize(Roles ="Admin")]
        
        public async Task<IActionResult> GetAllOwnersVerificationRequests() { 
        // from DB to Angular    
        var allVerificationRequests = await _unit.OwnerVerificationDocumentRepository.GetAllAsync();
            if (allVerificationRequests == null || !allVerificationRequests.Any())
                return Ok(new { Message = "No Requests Found" });
            Task<IEnumerable<OwnerWithUnitVerificationDTO>>? OwnersUnitsWaitingForVerification 
                = _unit.OwnerVerificationDocumentRepository.GetPendingOwnersWithUnitAsync();
            
            return Ok(OwnersUnitsWaitingForVerification);
        }
        
        [HttpPost("Respond")]
        [Authorize(Roles = "Admin")]
        public async Task RespondToVerificationRequest([FromBody] RespondToVerificationRequestDTO respondDTO)
        {
            Owner owner = await _unit.OwnerRepository.GetByIdAsync(respondDTO.OwnerId);
            owner.VerificationStatus = respondDTO.VerificationStatus;
            Unit unit = await _unit.UnitRepository.GetByIdAsync(respondDTO.UnitId);
            unit.VerificationStatus = respondDTO.VerificationStatus;
            await _unit.SaveAsync();
        }

    }
}
