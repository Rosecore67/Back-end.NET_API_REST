using Dot.Net.WebApi.Controllers;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Models;
using P7CreateRestApi.Models.DTOs.UserDTOs;
using System.Security.Claims;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<ILogger<UserController>> _mockLogger;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockUserManager.Object, _mockLogger.Object);
        }

        [TestMethod]
        public void GetAllUsers_ShouldReturnOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", UserName = "User1", Email = "user1@example.com", Fullname = "User One", Role = "User" },
                new User { Id = "2", UserName = "User2", Email = "user2@example.com", Fullname = "User Two", Role = "Admin" }
            };

            _mockUserManager.Setup(m => m.Users).Returns(users.AsQueryable());

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var userList = okResult.Value as List<UserDTO>;
            Assert.AreEqual(2, userList.Count);
        }

        [TestMethod]
        public async Task Create_ShouldReturnOk_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var userCreateDto = new UserCreateDTO
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                Fullname = "New User",
            };

            var identityResult = IdentityResult.Success;

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(identityResult);

            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), RoleCollection.User))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Create(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_WhenCreationFails()
        {
            // Arrange
            var userCreateDto = new UserCreateDTO
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                Fullname = "New User",
            };

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Creation failed" });

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(identityResult);

            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), RoleCollection.User))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Create(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnOk_WhenUserUpdatedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var userUpdateDto = new UserUpdateDTO
            {
                UserName = "UpdatedUser",
                Email = "updateduser@example.com",
                Fullname = "Updated User",
                Role = "Admin"
            };

            var existingUser = new User
            {
                Id = userId,
                UserName = "OldUser",
                Email = "olduser@example.com",
                Fullname = "Old User",
                Role = "User"
            };

            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(existingUser);

            _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.Update(userId, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult.Value);
            var updatedUser = okResult.Value as UserDTO;

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(userUpdateDto.UserName, updatedUser.UserName);
            Assert.AreEqual(userUpdateDto.Email, updatedUser.Email);
            Assert.AreEqual(userUpdateDto.Fullname, updatedUser.Fullname);
            Assert.AreEqual(userUpdateDto.Role, updatedUser.Role);

            _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = "1";
            var userUpdateDto = new UserUpdateDTO
            {
                UserName = "UpdatedUser",
                Email = "updateduser@example.com",
                Fullname = "Updated User",
                Role = "Admin"
            };

            // Configuration du mock pour FindByIdAsync
            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // utilisateur simulé avec des claims
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "2"), 
                    new Claim(ClaimTypes.Role, "Admin")        
                };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };

            // Act
            var result = await _controller.Update(userId, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteUser_ShouldReturnNoContent_WhenUserDeletedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var existingUser = new User { Id = userId, UserName = "OldUser" };

            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(m => m.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = "1";
            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
