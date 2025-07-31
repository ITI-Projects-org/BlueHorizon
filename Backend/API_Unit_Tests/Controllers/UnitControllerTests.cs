using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.UnitDTO;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class UnitControllerTests : BaseTestClass
    {
        private UnitController _controller = null!;
        private Mock<IPhotoService> _mockPhotoService = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _mockPhotoService = new Mock<IPhotoService>();
            _controller = new UnitController(
                MockUnitOfWork.Object,
                MockMapper.Object,
                _mockPhotoService.Object
            );
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkResultWithUnits()
        {
            // Arrange
            var units = new List<Unit>
            {
                new Unit 
                { 
                    Id = 1, 
                    Title = "Beach Villa", 
                    Description = "Beautiful beachfront villa",
                    UnitType = UnitType.Villa,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Sleeps = 6,
                    BasePricePerNight = 200,
                    Address = "123 Beach Road",
                    VillageName = "Coastal Village"
                },
                new Unit 
                { 
                    Id = 2, 
                    Title = "City Apartment", 
                    Description = "Modern city apartment",
                    UnitType = UnitType.Apartment,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Sleeps = 4,
                    BasePricePerNight = 100,
                    Address = "456 City Street",
                    VillageName = "Downtown"
                }
            };

            var unitDtos = new List<UnitDTO>
            {
                new UnitDTO 
                { 
                    Id = 1, 
                    Title = "Beach Villa", 
                    Description = "Beautiful beachfront villa",
                    UnitType = UnitType.Villa,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Sleeps = 6,
                    BasePricePerNight = 200,
                    Address = "123 Beach Road",
                    VillageName = "Coastal Village"
                },
                new UnitDTO 
                { 
                    Id = 2, 
                    Title = "City Apartment", 
                    Description = "Modern city apartment",
                    UnitType = UnitType.Apartment,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Sleeps = 4,
                    BasePricePerNight = 100,
                    Address = "456 City Street",
                    VillageName = "Downtown"
                }
            };

            MockUnitOfWork.Setup(u => u.UnitRepository.GetAllValidUnits())
                         .ReturnsAsync(units);
            MockMapper.Setup(m => m.Map<List<UnitDTO>>(units))
                     .Returns(unitDtos);
            MockUnitOfWork.Setup(u => u.UnitRepository.GetSingleImagePathByUnitId(1))
                         .ReturnsAsync("https://example.com/image1.jpg");
            MockUnitOfWork.Setup(u => u.UnitRepository.GetSingleImagePathByUnitId(2))
                         .ReturnsAsync("https://example.com/image2.jpg");

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedUnits = okResult.Value as List<UnitDTO>;
            Assert.IsNotNull(returnedUnits);
            Assert.AreEqual(2, returnedUnits.Count);
            Assert.AreEqual("Beach Villa", returnedUnits[0].Title);
            Assert.AreEqual(1, returnedUnits[0].UnitId);
        }

        [TestMethod]
        public async Task GetAll_EmptyList_ReturnsOkResultWithEmptyList()
        {
            // Arrange
            var units = new List<Unit>();
            var unitDtos = new List<UnitDTO>();

            MockUnitOfWork.Setup(u => u.UnitRepository.GetAllValidUnits())
                         .ReturnsAsync(units);
            MockMapper.Setup(m => m.Map<List<UnitDTO>>(units))
                     .Returns(unitDtos);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedUnits = okResult.Value as List<UnitDTO>;
            Assert.IsNotNull(returnedUnits);
            Assert.AreEqual(0, returnedUnits.Count);
        }

        [TestMethod]
        public async Task DeleteById_UnitNotFound_ReturnsNotFound()
        {
            // Arrange
            int unitId = 999;
            MockUnitOfWork.Setup(u => u.UnitRepository.GetUnitWithDetailsAsync(unitId))
                         .ReturnsAsync((Unit)null!);

            // Act
            var result = await _controller.DeleteById(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task DeleteById_ValidUnit_ReturnsOkResult()
        {
            // Arrange
            int unitId = 1;
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "owner123"),
                new Claim(ClaimTypes.Role, "Owner")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var unit = new Unit
            {
                Id = unitId,
                Title = "Test Unit",
                OwnerId = "owner123"
            };

            MockUnitOfWork.Setup(u => u.UnitRepository.GetUnitWithDetailsAsync(unitId))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.UnitRepository.DeleteByIdAsync(unitId))
                         .Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteById(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetAll_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            MockUnitOfWork.Setup(u => u.UnitRepository.GetAllValidUnits())
                         .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(
                async () => await _controller.GetAll()
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up resources if needed
        }
    }
}
