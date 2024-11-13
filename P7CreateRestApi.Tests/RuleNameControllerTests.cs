using Dot.Net.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Models.DTOs.RuleNameDTOs;
using P7CreateRestApi.Services.Interface;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class RuleNameControllerTests
    {
        private Mock<IRuleNameService> _mockRuleNameService;
        private Mock<ILogger<RuleNameController>> _mockLogger;
        private RuleNameController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRuleNameService = new Mock<IRuleNameService>();
            _mockLogger = new Mock<ILogger<RuleNameController>>();
            _controller = new RuleNameController(_mockRuleNameService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllRuleNames_ShouldReturnOkResult_WithListOfRuleNames()
        {
            // Arrange
            var ruleNames = new List<RuleName>
    {
        new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "{}", Template = "Template1", SqlStr = "SELECT *", SqlPart = "FROM Table" },
        new RuleName { Id = 2, Name = "Rule2", Description = "Description2", Json = "{}", Template = "Template2", SqlStr = "SELECT *", SqlPart = "FROM AnotherTable" }
    };
            _mockRuleNameService.Setup(s => s.GetAllRuleNamesAsync()).ReturnsAsync(ruleNames);

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var ruleNameList = okResult.Value as List<RuleNameDTO>;
            Assert.AreEqual(2, ruleNameList.Count);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received request to GET all rule names")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllRuleNames_ShouldLogWarning_WhenNoRuleNamesFound()
        {
            // Arrange
            _mockRuleNameService.Setup(s => s.GetAllRuleNamesAsync()).ReturnsAsync(new List<RuleName>());

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No rule names found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddRuleName_ShouldReturnCreatedAtAction_WhenRuleNameIsAdded()
        {
            // Arrange
            var ruleNameCreateDto = new RuleNameCreateDTO { Name = "New Rule", Description = "Description", Json = "{}", Template = "Template", SqlStr = "SELECT *", SqlPart = "FROM Table" };
            var newRuleName = new RuleName { Id = 1, Name = "New Rule", Description = "Description", Json = "{}", Template = "Template", SqlStr = "SELECT *", SqlPart = "FROM Table" };

            _mockRuleNameService.Setup(s => s.CreateRuleNameAsync(It.IsAny<RuleName>())).ReturnsAsync(newRuleName);

            // Act
            var result = await _controller.AddRuleName(ruleNameCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Created rule name with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddRuleName_ShouldLogWarning_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");
            var ruleNameCreateDto = new RuleNameCreateDTO { Description = "Description", Json = "{}", Template = "Template", SqlStr = "SELECT *", SqlPart = "FROM Table" };

            // Act
            var result = await _controller.AddRuleName(ruleNameCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid model state for AddRuleName request")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateRuleName_ShouldReturnOkResult_WhenRuleNameIsUpdated()
        {
            // Arrange
            int ruleNameId = 1;
            var ruleNameUpdateDto = new RuleNameUpdateDTO { Name = "Updated Rule", Description = "Updated Description", Json = "{}", Template = "Template Updated", SqlStr = "SELECT *", SqlPart = "FROM UpdatedTable" };
            var existingRuleName = new RuleName { Id = ruleNameId, Name = "Old Rule", Description = "Old Description", Json = "{}", Template = "Template Old", SqlStr = "SELECT *", SqlPart = "FROM OldTable" };

            _mockRuleNameService.Setup(s => s.GetRuleNameByIdAsync(ruleNameId)).ReturnsAsync(existingRuleName);
            _mockRuleNameService.Setup(s => s.UpdateRuleNameAsync(ruleNameId, It.IsAny<RuleName>())).ReturnsAsync(existingRuleName);

            // Act
            var result = await _controller.UpdateRuleName(ruleNameId, ruleNameUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updated rule name with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateRuleName_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            int ruleNameId = 1;
            var ruleNameUpdateDto = new RuleNameUpdateDTO { Name = "Updated Rule", Description = "Updated Description", Json = "{}", Template = "Template Updated", SqlStr = "SELECT *", SqlPart = "FROM UpdatedTable" };

            _mockRuleNameService.Setup(s => s.GetRuleNameByIdAsync(ruleNameId)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.UpdateRuleName(ruleNameId, ruleNameUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Rule name with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteRuleName_ShouldReturnNoContent_WhenRuleNameIsDeleted()
        {
            // Arrange
            int ruleNameId = 1;
            var existingRuleName = new RuleName { Id = ruleNameId, Name = "Rule to Delete", Description = "Description", Json = "{}", Template = "Template", SqlStr = "SELECT *", SqlPart = "FROM Table" };

            _mockRuleNameService.Setup(s => s.GetRuleNameByIdAsync(ruleNameId)).ReturnsAsync(existingRuleName);
            _mockRuleNameService.Setup(s => s.DeleteRuleNameAsync(ruleNameId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRuleName(ruleNameId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Deleted rule name with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteRuleName_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            int ruleNameId = 1;
            _mockRuleNameService.Setup(s => s.GetRuleNameByIdAsync(ruleNameId)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.DeleteRuleName(ruleNameId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Rule name with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
