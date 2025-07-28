using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Models;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class QRCodeControllerTests : BaseTestClass
    {
        private QrCodeController _controller = null!;
        private Mock<IPhotoService> _mockPhotoService = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _mockPhotoService = new Mock<IPhotoService>();
            _controller = new QrCodeController(
                MockMapper.Object,
                MockUnitOfWork.Object,
                _mockPhotoService.Object
            );
        }

        [TestMethod]
        public async Task CreateQr_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var qrDto = new QRDTO
            {
                BookingId = 1,
                TenantNationalId = "12345678901234",
                VillageName = "Test Village",
                UnitAddress = "123 Test Street",
                OwnerName = "John Owner",
                TenantName = "Jane Tenant"
            };

            var qrCode = new QRCode
            {
                Id = 1,
                BookingId = qrDto.BookingId,
                TenantNationalId = qrDto.TenantNationalId,
                VillageName = qrDto.VillageName,
                UnitAddress = qrDto.UnitAddress,
                OwnerName = qrDto.OwnerName,
                TenantName = qrDto.TenantName,
                QRCodeValue = "base64string"
            };

            MockMapper.Setup(m => m.Map<QRCode>(qrDto))
                     .Returns(qrCode);
            MockUnitOfWork.Setup(u => u.QRCodeRepository.AddAsync(It.IsAny<QRCode>()))
                         .ReturnsAsync(qrCode);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateQr(qrDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(nameof(_controller.GetQRCodeById), createdResult.ActionName);
        }

        [TestMethod]
        public async Task CreateQrCloud_ValidData_ReturnsCreatedResult()
        {
            // Arrange
            var qrDto = new QRDTO
            {
                BookingId = 1,
                TenantNationalId = "12345678901234",
                VillageName = "Test Village",
                UnitAddress = "123 Test Street",
                OwnerName = "John Owner",
                TenantName = "Jane Tenant"
            };

            var qrCode = new QRCode
            {
                Id = 1,
                BookingId = qrDto.BookingId,
                TenantNationalId = qrDto.TenantNationalId,
                VillageName = qrDto.VillageName,
                UnitAddress = qrDto.UnitAddress,
                OwnerName = qrDto.OwnerName,
                TenantName = qrDto.TenantName
            };

            var mockUploadResult = new ImageUploadResult
            {
                Url = new Uri("https://cloudinary.com/test-image.png"),
                Error = null
            };

            MockMapper.Setup(m => m.Map<QRCode>(qrDto))
                     .Returns(qrCode);
            _mockPhotoService.Setup(s => s.AddPhotoAsync(It.IsAny<IFormFile>()))
                           .ReturnsAsync(mockUploadResult);
            MockUnitOfWork.Setup(u => u.QRCodeRepository.AddAsync(It.IsAny<QRCode>()))
                         .ReturnsAsync(qrCode);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateQrCloud(qrDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task CreateQrCloud_PhotoServiceError_ReturnsBadRequest()
        {
            // Arrange
            var qrDto = new QRDTO
            {
                BookingId = 1,
                TenantNationalId = "12345678901234",
                VillageName = "Test Village",
                UnitAddress = "123 Test Street",
                OwnerName = "John Owner",
                TenantName = "Jane Tenant"
            };

            var qrCode = new QRCode
            {
                Id = 1,
                BookingId = qrDto.BookingId,
                TenantNationalId = qrDto.TenantNationalId
            };

            var mockUploadResult = new ImageUploadResult
            {
                Error = new CloudinaryDotNet.Actions.Error { Message = "Upload failed" }
            };

            MockMapper.Setup(m => m.Map<QRCode>(qrDto))
                     .Returns(qrCode);
            _mockPhotoService.Setup(s => s.AddPhotoAsync(It.IsAny<IFormFile>()))
                           .ReturnsAsync(mockUploadResult);

            // Act
            var result = await _controller.CreateQrCloud(qrDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetQRCodeById_ValidId_ReturnsFileResult()
        {
            // Arrange
            int qrId = 1;
            var qrCode = new QRCode
            {
                Id = qrId,
                QRCodeValue = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 })
            };

            MockUnitOfWork.Setup(u => u.QRCodeRepository.GetByIdAsync(qrId))
                         .ReturnsAsync(qrCode);

            // Act
            var result = await _controller.GetQRCodeById(qrId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(FileContentResult));
            var fileResult = result as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("image/png", fileResult.ContentType);
        }

        [TestMethod]
        public async Task GetQRCodeById_QRCodeNotFound_ThrowsException()
        {
            // Arrange
            int qrId = 999;
            MockUnitOfWork.Setup(u => u.QRCodeRepository.GetByIdAsync(qrId))
                         .ReturnsAsync((QRCode)null!);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _controller.GetQRCodeById(qrId)
            );
        }

        [TestMethod]
        public async Task CreateQr_ValidData_VerifiesMapperCall()
        {
            // Arrange
            var qrDto = new QRDTO { BookingId = 1 };
            var qrCode = new QRCode { Id = 1, BookingId = 1 };

            MockMapper.Setup(m => m.Map<QRCode>(qrDto))
                     .Returns(qrCode);
            MockUnitOfWork.Setup(u => u.QRCodeRepository.AddAsync(It.IsAny<QRCode>()))
                         .ReturnsAsync(qrCode);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.CreateQr(qrDto);

            // Assert
            MockMapper.Verify(m => m.Map<QRCode>(qrDto), Times.Once);
        }

        [TestMethod]
        public async Task CreateQrCloud_ValidData_VerifiesPhotoServiceCall()
        {
            // Arrange
            var qrDto = new QRDTO { BookingId = 1 };
            var qrCode = new QRCode { Id = 1, BookingId = 1 };

            var mockUploadResult = new ImageUploadResult
            {
                Url = new Uri("https://cloudinary.com/test-image.png"),
                Error = null
            };

            MockMapper.Setup(m => m.Map<QRCode>(qrDto))
                     .Returns(qrCode);
            _mockPhotoService.Setup(p => p.AddPhotoAsync(It.IsAny<IFormFile>()))
                           .ReturnsAsync(mockUploadResult);
            MockUnitOfWork.Setup(u => u.QRCodeRepository.AddAsync(It.IsAny<QRCode>()))
                         .ReturnsAsync(qrCode);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.CreateQrCloud(qrDto);

            // Assert
            _mockPhotoService.Verify(p => p.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [TestMethod]
        public async Task GetQRCodeById_ValidId_VerifiesRepositoryCall()
        {
            // Arrange
            int qrId = 1;
            var qrCode = new QRCode { Id = qrId, QRCodeValue = "SGVsbG8gV29ybGQ=" }; // Valid Base64 for "Hello World"
            
            MockUnitOfWork.Setup(u => u.QRCodeRepository.GetByIdAsync(qrId))
                         .ReturnsAsync(qrCode);

            // Act
            await _controller.GetQRCodeById(qrId);

            // Assert
            MockUnitOfWork.Verify(u => u.QRCodeRepository.GetByIdAsync(qrId), Times.Once);
        }

        [TestMethod]
        public async Task CreateQr_VerifiesRepositoryCall()
        {
            // Arrange
            var qrDto = new QRDTO { BookingId = 1 };
            var qrCode = new QRCode { Id = 1, BookingId = 1 };

            MockMapper.Setup(m => m.Map<QRCode>(qrDto))
                     .Returns(qrCode);
            MockUnitOfWork.Setup(u => u.QRCodeRepository.AddAsync(It.IsAny<QRCode>()))
                         .ReturnsAsync(qrCode);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.CreateQr(qrDto);

            // Assert
            MockUnitOfWork.Verify(u => u.QRCodeRepository.AddAsync(It.IsAny<QRCode>()), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetQRCodeById_InvalidId_ThrowsException()
        {
            // Arrange
            int invalidQrId = -1;
            MockUnitOfWork.Setup(u => u.QRCodeRepository.GetByIdAsync(invalidQrId))
                         .ReturnsAsync((QRCode)null!);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _controller.GetQRCodeById(invalidQrId)
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up resources if needed
        }
    }
}
