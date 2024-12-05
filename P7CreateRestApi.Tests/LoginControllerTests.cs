using Dot.Net.WebApi.Controllers;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Models.DTOs.UserDTOs;
using P7CreateRestApi.Models;
using P7CreateRestApi.Tests.Fakes;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class LoginControllerTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<ILogger<LoginController>> _mockLogger;
        private LoginController _controller;

        [TestInitialize]
        public void Setup()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);
            _mockLogger = new Mock<ILogger<LoginController>>();

            // Utilisation de FakeJwtUtils
            var fakeJwtUtils = new FakeJwtUtils();

            _controller = new LoginController(_mockUserManager.Object, fakeJwtUtils, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Register_ShouldReturnOk_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var userCreateDto = new UserCreateDTO
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Password@1234",
                Fullname = "Test User"
            };

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), userCreateDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), RoleCollection.User))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [TestMethod]
        public async Task Register_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "UserName is required");

            var userCreateDto = new UserCreateDTO
            {
                Email = "testuser@example.com",
                Password = "Password@1234",
                Fullname = "Test User"
            };

            // Act
            var result = await _controller.Register(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_ShouldReturnBadRequest_WhenUserCreationFails()
        {
            // Arrange
            var userCreateDto = new UserCreateDTO
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Password@1234",
                Fullname = "Test User"
            };

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), userCreateDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _controller.Register(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "Password@1234"
            };

            var user = new User
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Fullname = "Test User",
                Role = RoleCollection.User
            };

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);

            var responseValue = okResult.Value;
            var tokenProperty = responseValue.GetType().GetProperty("Token");
            Assert.IsNotNull(tokenProperty, "La réponse ne contient pas la propriété Token");

            var tokenValue = tokenProperty.GetValue(responseValue, null);
            Assert.AreEqual("fake-jwt-token", tokenValue);
        }

        [TestMethod]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            _mockUserManager.Setup(m => m.FindByNameAsync(loginModel.Username))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task Login_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Username", "Username is required");

            var loginModel = new LoginModel
            {
                Password = "Password@1234"
            };

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
