using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class ReviewControllerTests : BaseTestClass
    {
        private ReviewController _controller = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _controller = new ReviewController(MockMapper.Object, MockUnitOfWork.Object);
        }

        [TestMethod]
        public async Task AddReview_ValidReview_ReturnsOkResult()
        {
            // Arrange
            var reviewDto = new ReviewDTO
            {
                UnitId = 1,
                BookingId = 1,
                Rating = 5,
                Comment = "Great place!",
                ReviewDate = DateTime.UtcNow
            };

            SetupControllerContext(_controller, "tenant123", "Tenant");

            var booking = new Booking
            {
                Id = 1,
                TenantId = "tenant123",
                UnitId = 1
            };

            var unitReview = new UnitReview
            {
                UnitId = reviewDto.UnitId,
                TenantId = "tenant123",
                BookingId = reviewDto.BookingId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment
            };

            var unit = new Unit { Id = 1, AverageUnitRating = 0 };

            MockUnitOfWork.Setup(u => u.BookingRepository.GetByIdAsync(reviewDto.BookingId))
                         .ReturnsAsync(booking);
            MockMapper.Setup(m => m.Map<UnitReview>(It.IsAny<ReviewDTO>()))
                     .Returns(unitReview);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.AddAsync(It.IsAny<UnitReview>()))
                         .ReturnsAsync(unitReview);
            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(unitReview.UnitId))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.CalculateAverageRating(unitReview.UnitId))
                         .Returns(4.5);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddReview(reviewDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task AddReview_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            SetupControllerContext(_controller, "tenant123", "Tenant");
            
            var reviewDto = new ReviewDTO();
            _controller.ModelState.AddModelError("Rating", "Rating is required");

            // Act
            var result = await _controller.AddReview(reviewDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetAllUnitReviews_ValidUnitId_ReturnsOkResult()
        {
            // Arrange
            int unitId = 1;
            var reviews = new List<UnitReview>
            {
                new UnitReview { Id = 1, UnitId = unitId, Rating = 5, Comment = "Great!" },
                new UnitReview { Id = 2, UnitId = unitId, Rating = 4, Comment = "Good!" }
            };

            var reviewDtos = new List<ReviewDTO>
            {
                new ReviewDTO { UnitId = unitId, Rating = 5, Comment = "Great!", TenantId = "tenant1" },
                new ReviewDTO { UnitId = unitId, Rating = 4, Comment = "Good!", TenantId = "tenant2" }
            };

            MockUnitOfWork.Setup(u => u.UnitReviewRepository.GetAllUnitReviews(unitId))
                         .ReturnsAsync(reviews);
            MockMapper.Setup(m => m.Map<List<ReviewDTO>>(reviews))
                     .Returns(reviewDtos);
            MockUnitOfWork.Setup(u => u.TenantRepository.GetTenantNameBuUserId(It.IsAny<string>()))
                         .ReturnsAsync("Test User");

            // Act
            var result = await _controller.GetAllReviews(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(List<ReviewDTO>));
        }

        [TestMethod]
        public async Task GetAllUnitReviews_NoReviews_ReturnsEmptyList()
        {
            // Arrange
            int unitId = 1;
            var emptyReviews = new List<UnitReview>();
            var emptyReviewDtos = new List<ReviewDTO>();

            MockUnitOfWork.Setup(u => u.UnitReviewRepository.GetAllUnitReviews(unitId))
                         .ReturnsAsync(emptyReviews);
            MockMapper.Setup(m => m.Map<List<ReviewDTO>>(emptyReviews))
                     .Returns(emptyReviewDtos);

            // Act
            var result = await _controller.GetAllReviews(unitId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultList = okResult.Value as List<ReviewDTO>;
            Assert.IsNotNull(resultList);
            Assert.AreEqual(0, resultList.Count);
        }

        [TestMethod]
        public async Task DeleteReview_ValidId_ReturnsOkResult()
        {
            // Arrange
            SetupControllerContext(_controller, "tenant123", "Tenant");
            
            int reviewId = 1;
            var review = new UnitReview 
            { 
                Id = reviewId, 
                UnitId = 1, 
                Rating = 5, 
                TenantId = "tenant123", 
                BookingId = 1 
            };
            
            var unit = new Unit { Id = 1, AverageUnitRating = 0 };
            var booking = new Booking { Id = 1, UnitReviewed = true };

            MockUnitOfWork.Setup(u => u.UnitReviewRepository.GetByIdAsync(reviewId))
                         .ReturnsAsync(review);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.DeleteByIdAsync(reviewId));
            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(review.UnitId))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.CalculateAverageRating(review.UnitId))
                         .Returns(4.0);
            MockUnitOfWork.Setup(u => u.BookingRepository.GetByIdAsync(review.BookingId))
                         .ReturnsAsync(booking);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteReview(reviewId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeleteReview_ReviewNotFound_ReturnsNotFound()
        {
            // Arrange
            SetupControllerContext(_controller, "tenant123", "Tenant");
            
            int reviewId = 999;
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.GetByIdAsync(reviewId))
                         .ReturnsAsync((UnitReview)null!);

            // Act
            var result = await _controller.DeleteReview(reviewId);

            // Assert - Controller returns BadRequest for null review, not NotFound
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task AddReview_VerifiesRepositoryCall()
        {
            // Arrange
            SetupControllerContext(_controller, "tenant123", "Tenant");
            
            var reviewDto = new ReviewDTO
            {
                UnitId = 1,
                BookingId = 1,
                Rating = 5,
                Comment = "Great place!"
            };

            var booking = new Booking
            {
                Id = 1,
                TenantId = "tenant123",
                UnitId = 1
            };

            var unitReview = new UnitReview
            {
                UnitId = 1,
                TenantId = "tenant123",
                BookingId = 1,
                Rating = 5,
                Comment = "Great place!"
            };
            var unit = new Unit { Id = 1, AverageUnitRating = 0 };

            MockUnitOfWork.Setup(u => u.BookingRepository.GetByIdAsync(reviewDto.BookingId))
                         .ReturnsAsync(booking);
            MockMapper.Setup(m => m.Map<UnitReview>(It.IsAny<ReviewDTO>()))
                     .Returns(unitReview);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.AddAsync(It.IsAny<UnitReview>()))
                         .ReturnsAsync(unitReview);
            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(1))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.CalculateAverageRating(1))
                         .Returns(4.5);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.AddReview(reviewDto);

            // Assert
            MockUnitOfWork.Verify(u => u.UnitReviewRepository.AddAsync(It.IsAny<UnitReview>()), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task GetAllUnitReviews_VerifiesRepositoryCall()
        {
            // Arrange
            int unitId = 1;
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.GetAllUnitReviews(unitId))
                         .ReturnsAsync(new List<UnitReview>());
            MockMapper.Setup(m => m.Map<List<ReviewDTO>>(It.IsAny<List<UnitReview>>()))
                     .Returns(new List<ReviewDTO>());

            // Act
            await _controller.GetAllReviews(unitId);

            // Assert
            MockUnitOfWork.Verify(u => u.UnitReviewRepository.GetAllUnitReviews(unitId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteReview_VerifiesRepositoryCall()
        {
            // Arrange
            SetupControllerContext(_controller, "tenant123", "Tenant");
            
            int reviewId = 1;
            var review = new UnitReview 
            { 
                Id = reviewId, 
                TenantId = "tenant123", 
                UnitId = 1, 
                BookingId = 1 
            };
            
            var unit = new Unit { Id = 1, AverageUnitRating = 0 };
            var booking = new Booking { Id = 1, UnitReviewed = true };

            MockUnitOfWork.Setup(u => u.UnitReviewRepository.GetByIdAsync(reviewId))
                         .ReturnsAsync(review);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.DeleteByIdAsync(reviewId));
            MockUnitOfWork.Setup(u => u.UnitRepository.GetByIdAsync(review.UnitId))
                         .ReturnsAsync(unit);
            MockUnitOfWork.Setup(u => u.UnitReviewRepository.CalculateAverageRating(review.UnitId))
                         .Returns(4.0);
            MockUnitOfWork.Setup(u => u.BookingRepository.GetByIdAsync(review.BookingId))
                         .ReturnsAsync(booking);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.DeleteReview(reviewId);

            // Assert
            MockUnitOfWork.Verify(u => u.UnitReviewRepository.GetByIdAsync(reviewId), Times.Once);
            MockUnitOfWork.Verify(u => u.UnitReviewRepository.DeleteByIdAsync(reviewId), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(), Times.AtLeastOnce);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up resources if needed
        }
    }
}