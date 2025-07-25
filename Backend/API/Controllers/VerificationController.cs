
﻿using System.Security.Claims;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.VerificationDTO;
using API.Models;
using API.UnitOfWorks;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using CloudinaryDotNet.Actions;

namespace API.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
    
    public class VerificationController : ControllerBase
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        public IPhotoService _photoService { get; }

        public VerificationController(IMapper mapper, IUnitOfWork unit, IPhotoService photoService)
        {
            _mapper = mapper;
            _unit = unit;
            _photoService = photoService;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpPost("AddRequest")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> OwnerVerificationRequest([FromForm]OwnerWithUnitVerificationDTO ownerVerificationDTO){
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
            ImageUploadResult? FrontImageUploadResult = await _photoService.AddPhotoAsync(ownerVerificationDTO.FrontNationalIdDocument);
            ImageUploadResult? BackImageUploadResult = await _photoService.AddPhotoAsync(ownerVerificationDTO.BackNationalIdDocument);
            if (FrontImageUploadResult.Error != null || FrontImageUploadResult.Error != null)
                return BadRequest(FrontImageUploadResult.Error.Message);
            doc.FrontNationalIdDocumentPath = FrontImageUploadResult.Url.ToString();
            doc.BackNationalIdDocumentPath = BackImageUploadResult.Url.ToString();
            doc.UploadDate = DateTime.Now;
            _unit.OwnerVerificationDocumentRepository.AddAsync(doc);
            
            #endregion

            #region Verify Unit
            ImageUploadResult imageUploadResult = await _photoService.AddPhotoAsync(ownerVerificationDTO.ContractFile);
            
            if (imageUploadResult.Error != null )
                return BadRequest(imageUploadResult.Error.Message);

            Unit unit = _mapper.Map<Unit>(ownerVerificationDTO);
            unit.ContractPath = imageUploadResult.Url.ToString();
            
            _unit.UnitRepository.AddAsync(unit);
            //unit.UnitAmenities = ownerVerificationDTO.UnitAmenities;
            #endregion

            #region Add Amenities
            //if(ownerVerificationDTO.UnitAmenities !=null && ownerVerificationDTO.UnitAmenities.Any())
            //{
            //    foreach(var amenity in ownerVerificationDTO.UnitAmenities)
            //    {

            //    }
            //}
            await _unit.SaveAsync();

            if (ownerVerificationDTO.AmenityIds != null && ownerVerificationDTO.AmenityIds.Any())
            {
                foreach (var amenityId in ownerVerificationDTO.AmenityIds)
                {
                    var unitAmenity = new UnitAmenity
                    {
                        UnitId = unit.Id,
                        AmenityId = amenityId
                    };
                    await _unit.UnitAmenityRepository.AddAsync(unitAmenity);
                }
                await _unit.SaveAsync();
            }


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
            IEnumerable<OwnerWithUnitVerificationDTO>? OwnersUnitsWaitingForVerification 
                =await  _unit.OwnerVerificationDocumentRepository.GetPendingOwnersWithUnitAsync();
            
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
       
        [HttpGet("isVerified")]
        [Authorize]
        public async Task <IActionResult> isVerified (){
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Owner? user = await _unit.OwnerRepository.GetByIdAsync(userId);
            if(user == null)
                return NotFound(new{Message= "Owner not found"});
            var isVerified = user.VerificationStatus == VerificationStatus.Verified || user.VerificationStatus == VerificationStatus.Pending;
            return Ok(new {  isVerified = isVerified });
        }

    }
}

