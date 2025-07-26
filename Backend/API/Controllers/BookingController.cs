using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Security.Claims;
using API.DTOs.BookingDTOs;
using API.Models;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : Controller
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        
        public BookingController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin,Tenant")]
        public async Task<ActionResult> AddBooking(BookingDTO bookingdto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            if (bookingdto == null)
                return BadRequest(new { msg = "Booking is null" });

            var unit = await _unit.UnitRepository.GetByIdAsync(bookingdto.UnitId);
            if (unit == null)
                return NotFound(new { msg = "Unit not found" });

            // Date validations
            if (bookingdto.CheckInDate >= bookingdto.CheckOutDate)
            {
                return BadRequest(new { msg = "Check-out date must be after check-in date" });
            }

            if (bookingdto.CheckInDate < DateTime.Today)
            {
                return BadRequest(new { msg = "Check-in date cannot be in the past" });
            }

            // Check for booking conflicts using lazy loading
            if (!await _unit.BookingRepository.IsValidBooking(unit, bookingdto.CheckInDate, bookingdto.CheckOutDate))
            {
                return BadRequest(new { msg = "Sorry, your booking overlaps with another reservation" });
            }

            // Create and configure booking
            var booking = _mapper.Map<Booking>(bookingdto);
            booking.BookingDate = DateTime.Now;
            booking.TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            booking.PaymentStatus = PaymentStatus.Pending;
            booking.UnitReviewed = false;
            booking.OwnerReviewd = false;

            // Calculate pricing
            var numberOfDays = booking.CheckOutDate.Subtract(booking.CheckInDate).TotalDays;
            booking.OwnerPayoutAmount = unit.BasePricePerNight * (decimal)numberOfDays;
            booking.PlatformComission = booking.OwnerPayoutAmount * 0.15m;
            booking.TotalPrice = booking.OwnerPayoutAmount + booking.PlatformComission;

            try
            {
                await _unit.BookingRepository.AddAsync(booking);
                await _unit.SaveAsync();

                return Ok(new { 
                    msg = "Booking added successfully",
                    bookingId = booking.Id,
                    totalPrice = booking.TotalPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "An error occurred while creating the booking" });
            }
        }

        [HttpGet("booked-slots/{unitId}")]
        public async Task<ActionResult<BookedSlotsDTO>> GetBookedSlots(int unitId)
        {
            var unit = await _unit.UnitRepository.GetByIdAsync(unitId);
            if (unit == null)
                return NotFound(new { msg = "Unit not found" });

            var bookedSlots = _mapper.Map<BookedSlotsDTO>(unit);

            return Ok(bookedSlots);
        }

        // Optional: Add endpoint to get user's bookings
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Tenant")]
        public async Task<ActionResult> GetMyBookings()
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _unit.BookingRepository.GetAllAsync();
            var userBookings = bookings.Where(b => b.TenantId == tenantId).ToList();
            
            var bookingDTOs = _mapper.Map<List<BookingDTO>>(userBookings);
            return Ok(bookingDTOs);
        }
    }


}
