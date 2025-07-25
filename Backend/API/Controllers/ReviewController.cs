using System.Collections.Generic;
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
    public class ReviewController : ControllerBase
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        public ReviewController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        [HttpPost]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDTO)
        {
            reviewDTO.TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // reviewDTO.TenantId = "14fdfcb4-6ca5-405d-a018-0533504b8826";
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (reviewDTO == null)
                return BadRequest(new { Message = "Review data is null" });

            var booking = await _unit.BookingRepository.GetByIdAsync(reviewDTO.BookingId);
            if (booking == null)
                return NotFound(new { Message = "Booking not found" });
            if (booking.TenantId != reviewDTO.TenantId)
                return Unauthorized("You aren't Authoried to Review This Booking!");
            //if( booking.ReviewStatus != ReviewStatus.NotReviewed)
            //    return BadRequest("This Booking has already been reviewed");
            reviewDTO.ReviewDate = DateTime.Now;
            if (booking.CheckInDate > reviewDTO.ReviewDate)
                return BadRequest(new { Message = "you cannot add review before check-in date" });

            var review = _mapper.Map<UnitReview>(reviewDTO);
            await _unit.UnitReviewRepository.AddAsync(review);
            await _unit.SaveAsync();

            var unit = await _unit.UnitRepository.GetByIdAsync(review.UnitId);
            unit.AverageUnitRating = (float)_unit.UnitReviewRepository.CalculateAverageRating(review.UnitId);
            booking.UnitReviewed = true;
            await _unit.SaveAsync();
            return Ok(new { Message = "Review Added Succesfully ✅"});
        }
        [HttpGet("GetAllUnitReviews/{unitId}")]
        [Authorize(Roles = "Tenant,Owner,Admin")]
        public async Task<IActionResult> GetAllReviews(int unitId)
        {
            IEnumerable<UnitReview> unitReviews = await _unit.UnitReviewRepository.GetAllUnitReviews(unitId);
            IEnumerable<ReviewDTO>? unitreviewsDto = _mapper.Map<List<ReviewDTO>>(unitReviews);
            foreach (var unitReview in unitreviewsDto)
            {
                unitReview.TenantName = await _unit.TenantRepository.GetTenantNameBuUserId(unitReview.TenantId);
            }
            return Ok(unitreviewsDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Tenant,Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {

            UnitReview? review = await _unit.UnitReviewRepository.GetByIdAsync(id);

            if (review == null)
                return BadRequest(new { Message = "No Review Found!" });

            var TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (TenantId != review.TenantId)
                return Unauthorized("You aren't Authoried to Delete This Review!");

            int UnitId = review.UnitId;
            _unit.UnitReviewRepository.DeleteByIdAsync(id);
            await _unit.SaveAsync();

            var unit = await _unit.UnitRepository.GetByIdAsync(UnitId);
            unit.AverageUnitRating = (float)_unit.UnitReviewRepository.CalculateAverageRating(UnitId);
            Booking booking = await _unit.BookingRepository.GetByIdAsync(review.BookingId);
            booking.UnitReviewed = false;

            await _unit.SaveAsync();
            return Ok();
        }



    }
}
