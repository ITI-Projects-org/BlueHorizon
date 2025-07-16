using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Village_System.DTOs.VerificationDTO;
using Village_System.Models;
using Village_System.UnitOfWorks;

namespace Village_System.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
    public class VerificationController : Controller
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }

        public VerificationController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("OwnerVerificationRequest")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> OwnerVerificationRequest([FromBody]OwnerWithUnitVerificationDTO ownerVerificationDTO,UnitVerificationDTO unitVerificationDTO){
            #region Verify Owner
            var owner = await _unit.OwnerRepository.GetByIdAsync(ownerVerificationDTO.OwnerId);
            if (owner == null)
                return BadRequest("Owner not found");

            if (owner.VerificationStatus != VerificationStatus.NotVerified && owner.VerificationStatus != VerificationStatus.Rejected)
                return BadRequest("Owner already verified or pending");

            var ownerVerificationDocument = _mapper.Map<OwnerVerificationDocument>(ownerVerificationDTO);
            var res = await _unit.OwnerVerificationDocumentRepository.AddAsync(ownerVerificationDocument);


            owner.VerificationStatus = VerificationStatus.Pending; 
            owner.VerificationDate = DateTime.Now;
            #endregion
            #region Verify Unit
            // verify unit take logic from Momen
            #endregion
            await _unit.SaveAsync();
            return Ok();
        }
        [HttpGet("RespondOwnerVerification")]
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

    }
}
