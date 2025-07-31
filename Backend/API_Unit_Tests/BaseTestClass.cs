using API.Models;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace API_Unit_Tests
{
    [TestClass]
    public abstract class BaseTestClass
    {
        protected Mock<IUnitOfWork> MockUnitOfWork = null!;
        protected Mock<IMapper> MockMapper = null!;
        protected Mock<UserManager<ApplicationUser>> MockUserManager = null!;
        protected Mock<IConfiguration> MockConfiguration = null!;

        [TestInitialize]
        public virtual void Setup()
        {
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockMapper = new Mock<IMapper>();
            
            // Setup UserManager mock
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            MockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            MockConfiguration = new Mock<IConfiguration>();
        }

        protected ClaimsPrincipal CreateUserPrincipal(string userId, string role = "Tenant")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, "testuser@example.com")
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
        }

        protected void SetupControllerContext(ControllerBase controller, string userId, string role = "Tenant")
        {
            var user = CreateUserPrincipal(userId, role);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = user
                }
            };
        }
    }
}
