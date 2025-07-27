using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.AuthenticationDTO;
using API.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class AuthenticationControllerTests : BaseTestClass
    {
        private AuthenticationController _controller = null!;
        private Mock<IAuthService> _mockAuthService = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthenticationController(
                MockUserManager.Object,
                MockMapper.Object,
                MockConfiguration.Object,
                _mockAuthService.Object
            );
        }

        [TestMethod]
        public async Task Register_ValidTenantData_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = "Tenant"
            };

            var tenant = new Tenant
            {
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            // Mock controller URL helper
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                        .Returns("http://test.com/confirm");
            
            _controller.Url = mockUrlHelper.Object;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.Request.Scheme = "http";

            MockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
                          .ReturnsAsync((ApplicationUser)null!);
            MockMapper.Setup(m => m.Map<Tenant>(registerDto))
                     .Returns(tenant);
            MockUserManager.Setup(um => um.CreateAsync(It.IsAny<Tenant>(), registerDto.Password))
                          .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<Tenant>(), "Tenant"))
                          .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<Tenant>()))
                          .ReturnsAsync("test-token");
            _mockAuthService.Setup(s => s.SendEmailConfirmation(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Register_EmailAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Email = "existing@example.com",
                Username = "testuser",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = "Tenant"
            };

            var existingUser = new Tenant { Email = registerDto.Email };
            MockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
                          .ReturnsAsync(existingUser);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new Tenant { Email = loginDto.Email, UserName = "testuser", EmailConfirmed = true };
            MockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
                           .ReturnsAsync(user);
            MockUserManager.Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
                           .ReturnsAsync(true);
            MockUserManager.Setup(um => um.IsEmailConfirmedAsync(user))
                           .ReturnsAsync(true);
            MockUserManager.Setup(um => um.GetRolesAsync(user))
                           .ReturnsAsync(new List<string> { "Tenant" });

            _mockAuthService.Setup(s => s.GenerateTokens(It.IsAny<ApplicationUser>()))
                           .ReturnsAsync(("access_token", "refresh_token", DateTime.UtcNow.AddDays(30)));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var user = new Tenant { Email = loginDto.Email, UserName = "testuser", EmailConfirmed = true };
            MockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
                           .ReturnsAsync(user);
            MockUserManager.Setup(um => um.IsEmailConfirmedAsync(user))
                           .ReturnsAsync(true);
            MockUserManager.Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
                           .ReturnsAsync(false);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task Login_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            MockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
                           .ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task ChangePassword_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDTO
            {
                CurrentPassword = "oldPassword123",
                NewPassword = "newPassword123"
            };

            var user = new Tenant { Email = "test@example.com" };
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Email, "test@example.com")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            MockUserManager.Setup(um => um.GetUserAsync(userClaims))
                           .ReturnsAsync(user);
            MockUserManager.Setup(um => um.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword))
                           .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Refresh_ValidToken_ReturnsOkResult()
        {
            // Arrange
            var refreshRequest = new RefreshRequestDTO
            {
                AccessToken = "valid_access_token",
                RefreshToken = "valid_refresh_token"
            };

            var user = new Tenant 
            { 
                Id = "user123",
                Email = "test@example.com",
                RefreshToken = "valid_refresh_token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Email, "test@example.com")
            }));

            _mockAuthService.Setup(s => s.GetPrincipalFromExpiredToken(refreshRequest.AccessToken))
                           .Returns(userClaims);
            MockUserManager.Setup(um => um.FindByIdAsync("user123"))
                           .ReturnsAsync(user);
            MockUserManager.Setup(um => um.UpdateAsync(user))
                           .ReturnsAsync(IdentityResult.Success);
            _mockAuthService.Setup(s => s.GenerateTokens(user))
                           .ReturnsAsync(("new_access_token", "new_refresh_token", DateTime.UtcNow.AddDays(30)));

            // Act
            var result = await _controller.Refresh(refreshRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up resources if needed
        }
    }
}
