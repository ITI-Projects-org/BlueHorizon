using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.DTOs.VerificationDTO;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using API.Repositories.Interfaces;
using CloudinaryDotNet.Actions;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class VerificationControllerTests : BaseTestClass
    {
        private VerificationController _controller = null!;
        private Mock<IPhotoService> _mockPhotoService = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _mockPhotoService = new Mock<IPhotoService>();
            _controller = new VerificationController(
                MockMapper.Object,
                MockUnitOfWork.Object,
                _mockPhotoService.Object
            );

            // Setup controller context for claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-owner-id"),
                new Claim(ClaimTypes.Role, "Owner")
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
        public async Task OwnerVerificationRequest_ValidOwner_ReturnsOkResult()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var owner = new Owner
            {
                Id = ownerId,
                VerificationStatus = VerificationStatus.NotVerified
            };

            // Create mock files
            var mockFrontFile = new Mock<IFormFile>();
            var mockBackFile = new Mock<IFormFile>();
            var mockContractFile = new Mock<IFormFile>();
            
            mockFrontFile.Setup(f => f.Length).Returns(1024);
            mockBackFile.Setup(f => f.Length).Returns(1024);
            mockContractFile.Setup(f => f.Length).Returns(1024);

            var ownerVerificationDto = new OwnerWithUnitVerificationDTO
            {
                OwnerId = ownerId,
                NationalId = "123456789",
                BankAccountDetails = "Bank Account Info",
                Title = "Test Unit",
                Description = "Test Description",
                UnitType = UnitType.Apartment,
                Bedrooms = 2,
                Bathrooms = 1,
                Sleeps = 4,
                DistanceToSea = 100,
                BasePricePerNight = 100,
                Address = "Test Address",
                VillageName = "Test Village",
                FrontNationalIdDocument = mockFrontFile.Object,
                BackNationalIdDocument = mockBackFile.Object,
                ContractFile = mockContractFile.Object
            };

            var uploadResult = new CloudinaryDotNet.Actions.ImageUploadResult
            {
                Url = new Uri("http://test.com/image.jpg"),
                Error = null
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync(owner);
            _mockPhotoService.Setup(p => p.AddPhotoAsync(It.IsAny<IFormFile>()))
                           .ReturnsAsync(uploadResult);
            MockMapper.Setup(m => m.Map<OwnerVerificationDocument>(ownerVerificationDto))
                     .Returns(new OwnerVerificationDocument());
            MockMapper.Setup(m => m.Map<Unit>(ownerVerificationDto))
                     .Returns(new Unit());
            MockUnitOfWork.Setup(u => u.OwnerVerificationDocumentRepository.AddAsync(It.IsAny<OwnerVerificationDocument>()))
                         .ReturnsAsync(new OwnerVerificationDocument());
            MockUnitOfWork.Setup(u => u.UnitRepository.AddAsync(It.IsAny<Unit>()))
                         .ReturnsAsync(new Unit());
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.OwnerVerificationRequest(ownerVerificationDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task OwnerVerificationRequest_OwnerNotFound_ReturnsBadRequest()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var ownerVerificationDto = new OwnerWithUnitVerificationDTO
            {
                OwnerId = ownerId,
                NationalId = "123456789"
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync((Owner)null!);

            // Act
            var result = await _controller.OwnerVerificationRequest(ownerVerificationDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Owner not found", badRequestResult.Value);
        }

        [TestMethod]
        public async Task OwnerVerificationRequest_OwnerAlreadyVerified_ReturnsBadRequest()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var owner = new Owner
            {
                Id = ownerId,
                VerificationStatus = VerificationStatus.Verified
            };

            var ownerVerificationDto = new OwnerWithUnitVerificationDTO
            {
                OwnerId = ownerId,
                NationalId = "123456789"
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync(owner);

            // Act
            var result = await _controller.OwnerVerificationRequest(ownerVerificationDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Owner already verified or pending", badRequestResult.Value);
        }

        [TestMethod]
        public async Task OwnerVerificationRequest_OwnerPending_ReturnsBadRequest()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var owner = new Owner
            {
                Id = ownerId,
                VerificationStatus = VerificationStatus.Pending
            };

            var ownerVerificationDto = new OwnerWithUnitVerificationDTO
            {
                OwnerId = ownerId,
                NationalId = "123456789"
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync(owner);

            // Act
            var result = await _controller.OwnerVerificationRequest(ownerVerificationDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Owner already verified or pending", badRequestResult.Value);
        }

        [TestMethod]
        public async Task GetAllOwnersVerificationRequests_HasRequests_ReturnsOkResult()
        {
            // Arrange
            var verificationDocuments = new List<OwnerVerificationDocument>
            {
                new OwnerVerificationDocument { Id = 1, OwnerId = "owner1" },
                new OwnerVerificationDocument { Id = 2, OwnerId = "owner2" }
            };

            var pendingOwnersWithUnits = new List<OwnerWithUnitVerificationDTO>
            {
                new OwnerWithUnitVerificationDTO { OwnerId = "owner1", NationalId = "123456789" },
                new OwnerWithUnitVerificationDTO { OwnerId = "owner2", NationalId = "987654321" }
            };

            MockUnitOfWork.Setup(u => u.OwnerVerificationDocumentRepository.GetAllAsync())
                         .ReturnsAsync(verificationDocuments);
            MockUnitOfWork.Setup(u => u.OwnerVerificationDocumentRepository.GetPendingOwnersWithUnitAsync())
                         .ReturnsAsync(pendingOwnersWithUnits);

            // Act
            var result = await _controller.GetAllOwnersVerificationRequests();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedRequests = okResult.Value as IEnumerable<OwnerWithUnitVerificationDTO>;
            Assert.IsNotNull(returnedRequests);
            Assert.AreEqual(2, returnedRequests.Count());
        }

        [TestMethod]
        public async Task GetAllOwnersVerificationRequests_NoRequests_ReturnsOkWithMessage()
        {
            // Arrange
            var verificationDocuments = new List<OwnerVerificationDocument>();

            MockUnitOfWork.Setup(u => u.OwnerVerificationDocumentRepository.GetAllAsync())
                         .ReturnsAsync(verificationDocuments);

            // Act
            var result = await _controller.GetAllOwnersVerificationRequests();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var resultValue = okResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var messageProperty = resultType.GetProperty("Message");
            Assert.IsNotNull(messageProperty);
            Assert.AreEqual("No Requests Found", messageProperty.GetValue(resultValue));
        }

        [TestMethod]
        public async Task RespondToVerificationRequest_ValidRequest_UpdatesStatusSuccessfully()
        {
            // Arrange
            var respondDto = new RespondToVerificationRequestDTO
            {
                OwnerId = "owner-id",
                UnitId = 1,
                VerificationStatus = VerificationStatus.Verified
            };

            var owner = new Owner
            {
                Id = "owner-id",
                VerificationStatus = VerificationStatus.Pending
            };

            var unit = new Unit
            {
                Id = 1,
                VerificationStatus = VerificationStatus.Pending
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(respondDto.OwnerId))
                         .ReturnsAsync(owner);
            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(respondDto.UnitId))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.RespondToVerificationRequest(respondDto);

            // Assert
            Assert.AreEqual(VerificationStatus.Verified, owner.VerificationStatus);
            Assert.AreEqual(VerificationStatus.Verified, unit.VerificationStatus);
            MockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsVerified_VerifiedOwner_ReturnsTrue()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var owner = new Owner
            {
                Id = ownerId,
                VerificationStatus = VerificationStatus.Verified
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync(owner);

            // Act
            var result = await _controller.isVerified();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var resultValue = okResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var isVerifiedProperty = resultType.GetProperty("isVerified");
            Assert.IsNotNull(isVerifiedProperty);
            var isVerifiedValue = isVerifiedProperty.GetValue(resultValue);
            Assert.IsTrue((bool)isVerifiedValue);
        }

        [TestMethod]
        public async Task IsVerified_PendingOwner_ReturnsTrue()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var owner = new Owner
            {
                Id = ownerId,
                VerificationStatus = VerificationStatus.Pending
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync(owner);

            // Act
            var result = await _controller.isVerified();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var resultValue = okResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var isVerifiedProperty = resultType.GetProperty("isVerified");
            Assert.IsNotNull(isVerifiedProperty);
            var isVerifiedValue = isVerifiedProperty.GetValue(resultValue);
            Assert.IsTrue((bool)isVerifiedValue);
        }

        [TestMethod]
        public async Task IsVerified_NotVerifiedOwner_ReturnsFalse()
        {
            // Arrange
            var ownerId = "test-owner-id";
            var owner = new Owner
            {
                Id = ownerId,
                VerificationStatus = VerificationStatus.NotVerified
            };

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync(owner);

            // Act
            var result = await _controller.isVerified();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var resultValue = okResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var isVerifiedProperty = resultType.GetProperty("isVerified");
            Assert.IsNotNull(isVerifiedProperty);
            var isVerifiedValue = isVerifiedProperty.GetValue(resultValue);
            Assert.IsFalse((bool)isVerifiedValue);
        }

        [TestMethod]
        public async Task IsVerified_OwnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var ownerId = "test-owner-id";

            MockUnitOfWork.Setup(u => u.OwnerRepository.GetByIdAsync(ownerId))
                         .ReturnsAsync((Owner)null!);

            // Act
            var result = await _controller.isVerified();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            
            var resultValue = notFoundResult.Value;
            Assert.IsNotNull(resultValue);
            var resultType = resultValue.GetType();
            var messageProperty = resultType.GetProperty("Message");
            Assert.IsNotNull(messageProperty);
            Assert.AreEqual("Owner not found", messageProperty.GetValue(resultValue));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Controllers don't implement IDisposable
        }
    }
}
