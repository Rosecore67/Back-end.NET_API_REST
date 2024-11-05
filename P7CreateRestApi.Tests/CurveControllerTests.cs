using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Services.Interface;
using Dot.Net.WebApi.Controllers;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.CurveDTOs;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class CurveControllerTests
    {
        private Mock<ICurvePointService> _mockCurvePointService;
        private Mock<ILogger<CurveController>> _mockLogger;
        private CurveController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCurvePointService = new Mock<ICurvePointService>();
            _mockLogger = new Mock<ILogger<CurveController>>();
            _controller = new CurveController(_mockCurvePointService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllCurvePoints_ShouldReturnOkResult_WithListOfCurvePoints()
        {
            // Arrange
            var curvePoints = new List<CurvePoint>
    {
        new CurvePoint { Id = 1, Term = 5.0, CurvePointValue = 100.0 },
        new CurvePoint { Id = 2, Term = 10.0, CurvePointValue = 200.0 }
    };
            _mockCurvePointService.Setup(s => s.GetAllCurvePointsAsync()).ReturnsAsync(curvePoints);

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var curvePointList = okResult.Value as List<CurvePointDTO>;
            Assert.AreEqual(2, curvePointList.Count);
        }

        [TestMethod]
        public async Task GetAllCurvePoints_ShouldLogWarning_WhenNoCurvePointsFound()
        {
            // Arrange
            _mockCurvePointService.Setup(s => s.GetAllCurvePointsAsync()).ReturnsAsync(new List<CurvePoint>());

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No curves found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddCurvePoint_ShouldReturnCreatedAtAction_WhenCurvePointIsAdded()
        {
            // Arrange
            var curvePointCreateDto = new CurvePointCreateDTO { Term = 5.0, CurvePointValue = 100.0 };
            var newCurvePoint = new CurvePoint { Id = 1, Term = 5.0, CurvePointValue = 100.0 };

            _mockCurvePointService.Setup(s => s.CreateCurvePointAsync(It.IsAny<CurvePoint>())).ReturnsAsync(newCurvePoint);

            // Act
            var result = await _controller.AddCurvePoint(curvePointCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task AddCurvePoint_ShouldLogWarning_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Term", "Term is required");
            var curvePointCreateDto = new CurvePointCreateDTO { CurvePointValue = 100.0 };

            // Act
            var result = await _controller.AddCurvePoint(curvePointCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid model state")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateCurvePoint_ShouldReturnOkResult_WhenCurvePointIsUpdated()
        {
            // Arrange
            int curveId = 1;
            var curvePointUpdateDto = new CurvePointUpdateDTO { Term = 10.0, CurvePointValue = 150.0 };
            var existingCurvePoint = new CurvePoint { Id = curveId, Term = 5.0, CurvePointValue = 100.0 };

            _mockCurvePointService.Setup(s => s.GetCurvePointByIdAsync(curveId)).ReturnsAsync(existingCurvePoint);
            _mockCurvePointService.Setup(s => s.UpdateCurvePointAsync(curveId, It.IsAny<CurvePoint>())).ReturnsAsync(existingCurvePoint);

            // Act
            var result = await _controller.UpdateCurvePoint(curveId, curvePointUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task UpdateCurvePoint_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            int curveId = 1;
            var curvePointUpdateDto = new CurvePointUpdateDTO { CurveId = 1, Term = 10.0, CurvePointValue = 150.0 };

            _mockCurvePointService.Setup(s => s.GetCurvePointByIdAsync(curveId)).ReturnsAsync((CurvePoint)null);

            // Act
            var result = await _controller.UpdateCurvePoint(curveId, curvePointUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Curve with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteCurvePoint_ShouldReturnNoContent_WhenCurvePointIsDeleted()
        {
            // Arrange
            int curveId = 1;
            var existingCurvePoint = new CurvePoint { Id = curveId, Term = 5.0, CurvePointValue = 100.0 };

            _mockCurvePointService.Setup(s => s.GetCurvePointByIdAsync(curveId)).ReturnsAsync(existingCurvePoint);
            _mockCurvePointService.Setup(s => s.DeleteCurvePointAsync(curveId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCurvePoint(curveId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteCurvePoint_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            int curveId = 1;
            _mockCurvePointService.Setup(s => s.GetCurvePointByIdAsync(curveId)).ReturnsAsync((CurvePoint)null);

            // Act
            var result = await _controller.DeleteCurvePoint(curveId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Curve with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
