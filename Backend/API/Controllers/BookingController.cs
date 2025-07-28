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
using API.Repositories.Implementations;
using API.Repositories.Interfaces;

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

                // Send confirmation email to tenant
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

                // Send notification email to property owner
                if (unit.Owner != null && !string.IsNullOrEmpty(unit.Owner.Email))
                {
                    var ownerEmailBody = $@"
                        <h2>New Booking Notification</h2>
                        <p>Dear {unit.Owner.UserName},</p>
                        
                        <p>Great news! You have received a new booking for your property:</p>
                        
                        <div style='background-color: #e8f5e8; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #28a745;'>
                            <h3>Booking Information</h3>
                            <p><strong>Property:</strong> {unit.Title}</p>
                            <p><strong>Guest Name:</strong> {user?.UserName ?? "N/A"}</p>
                            <p><strong>Guest Email:</strong> {user?.Email ?? "N/A"}</p>
                            <p><strong>Booking Date:</strong> {booking.BookingDate:MMMM dd, yyyy}</p>
                            <p><strong>Check-in:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                            <p><strong>Check-out:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</p>
                            <p><strong>Number of Nights:</strong> {(int)numberOfDays}</p>
                            <p><strong>Your Payout:</strong> ${booking.OwnerPayoutAmount:F2}</p>
                            <p><strong>Platform Commission:</strong> ${booking.PlatformComission:F2}</p>
                            <p><strong>Total Booking Value:</strong> ${booking.TotalPrice:F2}</p>
                            <p><strong>Booking ID:</strong> #{booking.Id}</p>
                        </div>
                        
                        <p>Please ensure your property is ready for the guest's arrival.</p>
                        
                        <p>If you need to contact the guest or have any questions about this booking, please reach out to our support team.</p>
                        
                        <p>Thank you for being a valued BlueHorizon host!</p>
                        
                        <p>Best regards,<br>
                        The BlueHorizon Team</p>";

                    await _emailSender.SendEmailAsync(
                        unit.Owner.Email,
                        "New Booking Received - BlueHorizon",
                        ownerEmailBody);
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

        
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> GetMyBookings()
        {
            var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _unit.BookingRepository.GetAllAsync();
            var userBookings = bookings.Where(b => b.TenantId == tenantId).ToList();
            
            var bookingDTOs = _mapper.Map<List<BookingDTO>>(userBookings);
            if (bookingDTOs == null || !bookingDTOs.Any())
                return NotFound(new { msg = "No bookings found for this user" });
            foreach (var booking in bookingDTOs)
            {
                var QrCode=( await _unit.QRCodeRepository.GetQrCodeByBookingId(booking.Id));
                booking.QrCodeUrl = QrCode?.ImagePath??"";
            }
            return Ok(bookingDTOs);
        }

        [HttpDelete("delete/{bookingId}")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            try
            {
                // Get the current tenant ID
                var tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Find the booking
                var booking = await _unit.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                    return NotFound(new { msg = "Booking not found" });

                // Verify the booking belongs to the current tenant
                if (booking.TenantId != tenantId)
                    return Unauthorized(new { msg = "You are not authorized to delete this booking" });

                // Check if the booking can be cancelled (e.g., not too close to check-in date)
                var daysDifference = (booking.CheckInDate - DateTime.Today).TotalDays;
                if (daysDifference < 1) // Can't cancel if check-in is today or has passed
                {
                    return BadRequest(new { msg = "Cannot cancel booking. Check-in date is today or has passed." });
                }

                // Get unit details for email notification
                var unit = await _unit.UnitRepository.GetByIdAsync(booking.UnitId);
                
                // Delete the booking (QR codes and access permissions will cascade delete)
                await _unit.BookingRepository.DeleteByIdAsync(bookingId);
                await _unit.SaveAsync();

                // Send cancellation email to tenant
                var user = await _userManager.FindByIdAsync(tenantId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var emailBody = $@"
                        <h2>Booking Cancellation Confirmation</h2>
                        <p>Dear {user.UserName},</p>
                        
                        <p>This email confirms that your booking has been successfully cancelled:</p>
                        
                        <div style='background-color: #ffe6e6; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #dc3545;'>
                            <h3>Cancelled Booking Details</h3>
                            <p><strong>Property:</strong> {unit?.Title ?? "N/A"}</p>
                            <p><strong>Booking ID:</strong> #{booking.Id}</p>
                            <p><strong>Check-in Date:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                            <p><strong>Check-out Date:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</p>
                            <p><strong>Cancellation Date:</strong> {DateTime.Now:MMMM dd, yyyy}</p>
                            <p><strong>Refund Amount:</strong> ${booking.TotalPrice:F2}</p>
                        </div>
                        
                        <p>Your refund will be processed within 3-5 business days and will be credited back to your original payment method.</p>
                        
                        <p>If you have any questions about this cancellation or your refund, please don't hesitate to contact our support team.</p>
                        
                        <p>We're sorry to see you cancel and hope to serve you again in the future.</p>
                        
                        <p>Best regards,<br>
                        The BlueHorizon Team</p>";

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Booking Cancellation Confirmation - BlueHorizon",
                        emailBody);
                }

                // Send cancellation notification to property owner
                if (unit?.Owner != null && !string.IsNullOrEmpty(unit.Owner.Email))
                {
                    var ownerEmailBody = $@"
                        <h2>Booking Cancellation Notice</h2>
                        <p>Dear {unit.Owner.UserName},</p>
                        
                        <p>We wanted to inform you that a booking for your property has been cancelled:</p>
                        
                        <div style='background-color: #fff3cd; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #ffc107;'>
                            <h3>Cancelled Booking Information</h3>
                            <p><strong>Property:</strong> {unit.Title}</p>
                            <p><strong>Guest Name:</strong> {user?.UserName ?? "N/A"}</p>
                            <p><strong>Booking ID:</strong> #{booking.Id}</p>
                            <p><strong>Check-in Date:</strong> {booking.CheckInDate:MMMM dd, yyyy}</p>
                            <p><strong>Check-out Date:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</p>
                            <p><strong>Cancellation Date:</strong> {DateTime.Now:MMMM dd, yyyy}</p>
                            <p><strong>Lost Revenue:</strong> ${booking.OwnerPayoutAmount:F2}</p>
                        </div>
                        
                        <p>This time slot is now available for new bookings. You may want to update your property calendar or promotional offers.</p>
                        
                        <p>If you have any questions about this cancellation, please reach out to our support team.</p>
                        
                        <p>Thank you for your understanding.</p>
                        
                        <p>Best regards,<br>
                        The BlueHorizon Team</p>";

                    await _emailSender.SendEmailAsync(
                        unit.Owner.Email,
                        "Booking Cancellation Notice - BlueHorizon",
                        ownerEmailBody);
                }

                return Ok(new { 
                    msg = "Booking cancelled successfully. Confirmation emails have been sent.",
                    refundAmount = booking.TotalPrice,
                    cancellationDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "An error occurred while cancelling the booking" });
            }
        }
    }


}
