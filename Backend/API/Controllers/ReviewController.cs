using System.Security.Claims;
using API.DTOs;
using API.Models;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        public ReviewController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        [HttpPost]
        [Authorize(Roles ="Tenant")]
        public async Task <IActionResult> AddReview([FromBody] ReviewDTO reviewDTO)
        {
            reviewDTO.TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!ModelState.IsValid)
                return BadRequest(ModelState );
            if(reviewDTO == null)
                return BadRequest(new { Message = "Review data is null" });

            var booking = await  _unit.BookingRepository.GetByIdAsync(reviewDTO.BookingId);
            if (booking == null)
                return NotFound(new { Message = "Booking not found" });
            if (booking.TenantId != reviewDTO.TenantId) 
                return Unauthorized("You aren't Authoried to Review This Booking!");
            //if( booking.ReviewStatus != ReviewStatus.NotReviewed)
            //    return BadRequest("This Booking has already been reviewed");
            reviewDTO.ReviewDate = DateTime.Now; 
            if(booking.CheckInDate > reviewDTO.ReviewDate   )
                return BadRequest(new { Message = "you cannot add review before check-in date" });

            var review = _mapper.Map<UnitReview>(reviewDTO);
            await _unit.UnitReviewRepository.AddAsync(review);
            await _unit.SaveAsync();

            return Ok(new { Message = "Review Added Succesfully ✅" });
        }
    }
}
