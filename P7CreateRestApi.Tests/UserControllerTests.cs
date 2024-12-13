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
        public void GetAllUsers_ShouldReturnInternalError_OnException()
        {
            // Arrange
            _mockUserManager.Setup(m => m.Users).Throws(new Exception("Simulated exception"));

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while retrieving users")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
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
        public async Task Create_ShouldReturnBadRequest_WhenRoleAssignmentFails()
        {
            // Arrange
            var userCreateDto = new UserCreateDTO
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Password@1234",
                Fullname = "Test User"
            };

            var user = new User
            {
                UserName = userCreateDto.UserName,
                Email = userCreateDto.Email,
                Fullname = userCreateDto.Fullname
            };

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), RoleCollection.User))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to assign role" }));

            // Act
            var result = await _controller.Create(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            var errors = badRequestResult.Value as IEnumerable<IdentityError>;
            Assert.IsNotNull(errors);
            Assert.IsTrue(errors.Any(e => e.Description == "Failed to assign role"));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to assign role to user")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Create_ShouldReturnInternalError_OnException()
        {
            // Arrange
            var userCreateDto = new UserCreateDTO
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Password@1234",
                Fullname = "Test User"
            };

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.Create(userCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while creating a new user")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
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
        public async Task Update_ShouldReturnForbid_WhenUserIsUnauthorized()
        {
            // Arrange
            var userId = "1";
            var userUpdateDto = new UserUpdateDTO
            {
                UserName = "UpdatedUser",
                Email = "updateduser@example.com",
                Fullname = "Updated User",
                Role = "User"
            };

            var mockClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "2")
    };
            var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(mockClaims));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockPrincipal }
            };

            // Act
            var result = await _controller.Update(userId, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unauthorized attempt to update user")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Update_ShouldReturnInternalError_WhenUpdateFails()
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
                UserName = "ExistingUser",
                Email = "existinguser@example.com",
                Fullname = "Existing User",
                Role = "User"
            };

            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, "Admin")
    };
            var identity = new ClaimsIdentity(claims, "mock");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.Update(userId, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while updating the user.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to update user with ID")),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Update_ShouldReturnInternalError_OnException()
        {
            // Arrange
            var userId = "1";
            var userUpdateDto = new UserUpdateDTO
            {
                UserName = "UpdatedUser",
                Email = "updateduser@example.com",
                Fullname = "Updated User",
                Role = "User"
            };

            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.Update(userId, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while updating the user")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
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

        [TestMethod]
        public async Task DeleteUser_ShouldReturnInternalError_WhenDeleteFails()
        {
            // Arrange
            var userId = "1";
            var existingUser = new User
            {
                Id = userId,
                UserName = "ExistingUser",
                Email = "existinguser@example.com",
                Fullname = "Existing User",
                Role = "User"
            };

            // Simule l'utilisateur trouvé
            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(existingUser);

            // Simule l'échec de suppression
            _mockUserManager.Setup(m => m.DeleteAsync(existingUser))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while deleting the user.", objectResult.Value);

            // Vérification du log
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to delete user with ID")),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteUser_ShouldReturnInternalError_OnException()
        {
            // Arrange
            var userId = "1";

            // Simule une exception dans FindByIdAsync
            _mockUserManager.Setup(m => m.FindByIdAsync(userId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            // Vérification du log
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while deleting the user with ID")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
