using API.Controllers;
using API.DTOs.AmenityDTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class AmenityControllerTests : BaseTestClass
    {
        private AmenityController? _controller;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _controller = new AmenityController(MockMapper.Object, MockUnitOfWork.Object);
        }

        [TestMethod]
        public async Task GetAllAmenities_ReturnsOkResultWithAmenities()
        {
            // Arrange
            var amenities = new List<Amenity>
            {
                new Amenity { Id = 1, Name = AmenityName.WIFI },
                new Amenity { Id = 2, Name = AmenityName.PoolAccess },
                new Amenity { Id = 3, Name = AmenityName.AC }
            };

            var amenityDTOs = new List<AmenityDTO>
            {
                new AmenityDTO { Id = 1, Name = "WIFI" },
                new AmenityDTO { Id = 2, Name = "PoolAccess" },
                new AmenityDTO { Id = 3, Name = "AC" }
            };

            MockUnitOfWork.Setup(u => u.AmenityRepository.GetAllAsync())
                         .ReturnsAsync(amenities);
            MockMapper.Setup(m => m.Map<List<AmenityDTO>>(amenities))
                     .Returns(amenityDTOs);

            // Act
            var result = await _controller!.GetAllAmenities();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedAmenities = okResult.Value as List<AmenityDTO>;
            Assert.IsNotNull(returnedAmenities);
            Assert.AreEqual(3, returnedAmenities.Count);
            Assert.AreEqual("WIFI", returnedAmenities[0].Name);
        }

        [TestMethod]
        public async Task GetAllAmenities_EmptyList_ReturnsOkResultWithEmptyList()
        {
            // Arrange
            var amenities = new List<Amenity>();
            var amenityDTOs = new List<AmenityDTO>();

            MockUnitOfWork.Setup(u => u.AmenityRepository.GetAllAsync())
                         .ReturnsAsync(amenities);
            MockMapper.Setup(m => m.Map<List<AmenityDTO>>(amenities))
                     .Returns(amenityDTOs);

            // Act
            var result = await _controller!.GetAllAmenities();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedAmenities = okResult.Value as List<AmenityDTO>;
            Assert.IsNotNull(returnedAmenities);
            Assert.AreEqual(0, returnedAmenities.Count);
        }

        [TestMethod]
        public async Task GetAllAmenities_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            MockUnitOfWork.Setup(u => u.AmenityRepository.GetAllAsync())
                         .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(
                async () => await _controller!.GetAllAmenities()
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Controllers don't implement IDisposable
        }
    }
}
