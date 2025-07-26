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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : Controller
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        public IConfiguration _config { get; set; }
        public IEmailSender _emailSender { get; set; }
        public UserManager<ApplicationUser> _userManager { get; }

        public BookingController(IMapper mapper, IUnitOfWork unit, UserManager<ApplicationUser> userManager, IConfiguration config, IEmailSender emailSender)
        {
            _mapper = mapper;
            _unit = unit;
            _userManager = userManager;
            _config = config;
            _emailSender = emailSender;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin,Tenant")]
        public async Task<IActionResult> AddBooking(BookingDTO bookingdto)
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

                // Send confirmation email
                var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(tenantId);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var redirectUrl = $"{_config["ClientApp:BaseUrl"]}/my-bookings";

                    var emailBody = $@"
                        <h2>Booking Confirmation</h2>
                        <p>Dear {user.UserName},</p>
                        
                        <p>Thank you for your booking! This email confirms your reservation details:</p>
                        
                        <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                            <h3>Booking Details</h3>
                            <p><strong>Property:</strong> {unit.Title}</p>
                            <p><strong>Booking Date:</strong> {booking.BookingDate:MMMM dd, yyyy}</p>
                            <p><strong>Check-in:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                            <p><strong>Check-out:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</p>
                            <p><strong>Number of Nights:</strong> {(int)numberOfDays}</p>
                            <p><strong>Total Amount:</strong> ${booking.TotalPrice:F2}</p>
                            <p><strong>Booking ID:</strong> #{booking.Id}</p>
                        </div>
                        
                        <p>You can view all your bookings and manage your reservations by <a href='{redirectUrl}' style='color: #007bff;'>clicking here</a>.</p>
                        
                        <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                        
                        <p>Thank you for choosing BlueHorizon!</p>
                        
                        <p>Best regards,<br>
                        The BlueHorizon Team</p>";

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Booking Confirmation - BlueHorizon",
                        emailBody);
                }

                return Ok(new { 
                    msg = "Booking added successfully! A confirmation email has been sent.",
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
        public async Task<IActionResult> GetBookedSlots(int unitId)
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
        public async Task<IActionResult> GetMyBookings()
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _unit.BookingRepository.GetAllAsync();
            var userBookings = bookings.Where(b => b.TenantId == tenantId).ToList();
            
            var bookingDTOs = _mapper.Map<List<BookingDTO>>(userBookings);

            return Ok(bookingDTOs);
        }
    }


}
