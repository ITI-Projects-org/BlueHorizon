using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.DTOs.BookingDTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using API.DTOs.UnitDTO;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class BookingControllerTests : BaseTestClass
    {
        private BookingController _controller = null!;
        private Mock<IEmailSender> _mockEmailSender = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _mockEmailSender = new Mock<IEmailSender>();
            _controller = new BookingController(
                MockMapper.Object,
                MockUnitOfWork.Object,
                MockUserManager.Object,
                MockConfiguration.Object,
                _mockEmailSender.Object
            );

            // Setup controller context for claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "Tenant")
            };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [TestMethod]
        public async Task AddBooking_ValidBooking_ReturnsOkResult()
        {
            // Arrange
            var bookingDto = new BookingDTO
            {
                UnitId = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuests = 2,
                TotalPrice = 200
            };

            var unit = new Unit
            {
                Id = 1,
                Title = "Test Unit",
                VerificationStatus = VerificationStatus.Verified,
                BasePricePerNight = 100
            };

            var booking = new Booking
            {
                Id = 1,
                UnitId = bookingDto.UnitId,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate
            };

            var user = new Tenant
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@example.com"
            };

            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(bookingDto.UnitId))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.BookingRepository.IsValidBooking(unit, bookingDto.CheckInDate, bookingDto.CheckOutDate))
                         .ReturnsAsync(true);
            MockMapper.Setup(m => m.Map<Booking>(bookingDto))
                     .Returns(booking);
            MockUnitOfWork.Setup(u => u.BookingRepository.AddAsync(It.IsAny<Booking>()))
                         .ReturnsAsync(booking);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);
            MockUserManager.Setup(um => um.FindByIdAsync("test-user-id"))
                          .ReturnsAsync(user);
            _mockEmailSender.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddBooking(bookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task AddBooking_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var bookingDto = new BookingDTO();
            _controller.ModelState.AddModelError("UnitId", "Required");

            // Act
            var result = await _controller.AddBooking(bookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task AddBooking_NullBooking_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddBooking(null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            
            var resultValue = badRequestResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var msgProperty = resultType.GetProperty("msg");
            Assert.IsNotNull(msgProperty);
            Assert.AreEqual("Booking is null", msgProperty.GetValue(resultValue));
        }

        [TestMethod]
        public async Task AddBooking_UnitNotFound_ReturnsNotFound()
        {
            // Arrange
            var bookingDto = new BookingDTO
            {
                UnitId = 999,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3)
            };

            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(bookingDto.UnitId))
                         .ReturnsAsync((Unit)null!);

            // Act
            var result = await _controller.AddBooking(bookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            
            var resultValue = notFoundResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var msgProperty = resultType.GetProperty("msg");
            Assert.IsNotNull(msgProperty);
            Assert.AreEqual("Unit not found", msgProperty.GetValue(resultValue));
        }

        [TestMethod]
        public async Task GetBookedSlots_ValidUnitId_ReturnsOkResult()
        {
            // Arrange
            var unitId = 1;
            var unit = new Unit
            {
                Id = unitId,
                Title = "Test Unit"
            };

            var bookedSlotsDto = new BookedSlotsDTO
            {
                BookingSlots = new List<BookingSlotDTO>()
            };

            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(unitId))
                         .ReturnsAsync(unit);
            MockMapper.Setup(m => m.Map<BookedSlotsDTO>(unit))
                     .Returns(bookedSlotsDto);

            // Act
            var result = await _controller.GetBookedSlots(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(bookedSlotsDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetBookedSlots_UnitNotFound_ReturnsNotFound()
        {
            // Arrange
            var unitId = 999;

            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(unitId))
                         .ReturnsAsync((Unit)null!);

            // Act
            var result = await _controller.GetBookedSlots(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            
            var resultValue = notFoundResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var msgProperty = resultType.GetProperty("msg");
            Assert.IsNotNull(msgProperty);
            Assert.AreEqual("Unit not found", msgProperty.GetValue(resultValue));
        }

        [TestMethod]
        public async Task GetMyBookings_HasBookings_ReturnsOkResult()
        {
            // Arrange
            var tenantId = "test-user-id";
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, TenantId = tenantId, UnitId = 1 },
                new Booking { Id = 2, TenantId = "other-user", UnitId = 2 },
                new Booking { Id = 3, TenantId = tenantId, UnitId = 3 }
            };

            var userBookings = bookings.Where(b => b.TenantId == tenantId).ToList();
            var bookingDTOs = new List<BookingDTO>
            {
                new BookingDTO { Id = 1, TenantId = tenantId, UnitId = 1, QrCodeUrl = "" },
                new BookingDTO { Id = 3, TenantId = tenantId, UnitId = 3, QrCodeUrl = "" }
            };

            MockUnitOfWork.Setup(u => u.BookingRepository.GetAllAsync())
                         .ReturnsAsync(bookings);
            MockMapper.Setup(m => m.Map<List<BookingDTO>>(It.IsAny<List<Booking>>()))
                     .Returns(bookingDTOs);
            MockUnitOfWork.Setup(u => u.QRCodeRepository.GetQrCodeByBookingId(It.IsAny<int>()))
                         .ReturnsAsync((QRCode)null!);

            // Act
            var result = await _controller.GetMyBookings();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedBookings = okResult.Value as List<BookingDTO>;
            Assert.IsNotNull(returnedBookings);
            Assert.AreEqual(2, returnedBookings.Count);
        }

        [TestMethod]
        public async Task GetMyBookings_NoBookings_ReturnsNotFound()
        {
            // Arrange
            var bookings = new List<Booking>();
            var bookingDTOs = new List<BookingDTO>();

            MockUnitOfWork.Setup(u => u.BookingRepository.GetAllAsync())
                         .ReturnsAsync(bookings);
            MockMapper.Setup(m => m.Map<List<BookingDTO>>(It.IsAny<List<Booking>>()))
                     .Returns(bookingDTOs);

            // Act
            var result = await _controller.GetMyBookings();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            
            var resultValue = notFoundResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var msgProperty = resultType.GetProperty("msg");
            Assert.IsNotNull(msgProperty);
            Assert.AreEqual("No bookings found for this user", msgProperty.GetValue(resultValue));
        }

        [TestMethod]
        public async Task AddBooking_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            BookingDTO? nullRequest = null;

            // Act
            var result = await _controller.AddBooking(nullRequest!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task AddBooking_InvalidModelStateCheck_ReturnsBadRequest()
        {
            // Arrange
            var bookingDto = new BookingDTO();
            _controller.ModelState.AddModelError("UnitId", "Unit ID is required");

            // Act
            var result = await _controller.AddBooking(bookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetMyBookings_VerifiesRepositoryCall()
        {
            // Arrange
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-tenant-id"),
                new Claim(ClaimTypes.Role, "Tenant")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            MockUnitOfWork.Setup(u => u.BookingRepository.GetAllAsync())
                         .ReturnsAsync(new List<Booking>());
            MockMapper.Setup(m => m.Map<List<BookingDTO>>(It.IsAny<List<Booking>>()))
                     .Returns(new List<BookingDTO>());

            // Act
            await _controller.GetMyBookings();

            // Assert
            MockUnitOfWork.Verify(u => u.BookingRepository.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetBookedSlots_ValidUnitIdCheck_ReturnsOkResult()
        {
            // Arrange
            int unitId = 1;
            var unit = new Unit { Id = unitId, Title = "Test Unit" };
            var bookedSlotsDto = new BookedSlotsDTO();
            
            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(unitId))
                         .ReturnsAsync(unit);
            MockMapper.Setup(m => m.Map<BookedSlotsDTO>(unit))
                     .Returns(bookedSlotsDto);

            // Act
            var result = await _controller.GetBookedSlots(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Controllers don't implement IDisposable
        }
    }
}
