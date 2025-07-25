using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Security.Claims;
using API.DTOs.BookingDTOs;
using API.Models;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task< ActionResult >AddBooking(BookingDTO bookingdto)
        {
            if(bookingdto==null)
                return BadRequest("bookinng is null");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var unit = await _unit.UnitRepository.GetByIdAsync(bookingdto.UnitId);
            if(unit==null)
                return NotFound(new { Message="Unit not found" });

            var booking = _mapper.Map<Booking>(bookingdto);
            booking.BookingDate = DateTime.Now; 
            booking.TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            booking.PaymentStatus = PaymentStatus.Pending;
            
            var numberOfDays = booking.CheckOutDate.Subtract(booking.CheckInDate).TotalDays;
            booking.OwnerPayoutAmount = unit.BasePricePerNight * (int) numberOfDays;
            booking.PlatformComission = booking.OwnerPayoutAmount * .15m;
            booking.TotalPrice = booking.OwnerPayoutAmount+ booking.PlatformComission;

            await _unit.BookingRepository.AddAsync(booking);
            await _unit.SaveAsync();
            
            return Ok(new { Message = "Booking added successfully" });
        }

    }
}
