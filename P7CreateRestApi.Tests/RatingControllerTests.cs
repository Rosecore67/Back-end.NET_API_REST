using Dot.Net.WebApi.Controllers;
using Dot.Net.WebApi.Controllers.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Models.DTOs.RatingDTOs;
using P7CreateRestApi.Services.Interface;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class RatingControllerTests
    {
        private Mock<IRatingService> _mockRatingService;
        private Mock<ILogger<RatingController>> _mockLogger;
        private RatingController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRatingService = new Mock<IRatingService>();
            _mockLogger = new Mock<ILogger<RatingController>>();
            _controller = new RatingController(_mockRatingService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllRatings_ShouldReturnOkResult_WithListOfRatings()
        {
            // Arrange
            var ratings = new List<Rating>
    {
        new Rating { Id = 1, MoodysRating = "A", SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 },
        new Rating { Id = 2, MoodysRating = "B", SandPRating = "BB", FitchRating = "BBB", OrderNumber = 2 }
    };
            _mockRatingService.Setup(s => s.GetAllRatingsAsync()).ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var ratingList = okResult.Value as List<RatingDTO>;
            Assert.AreEqual(2, ratingList.Count);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received request to GET all ratings")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllRatings_ShouldLogWarning_WhenNoRatingsFound()
        {
            // Arrange
            _mockRatingService.Setup(s => s.GetAllRatingsAsync()).ReturnsAsync(new List<Rating>());

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No ratings found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddRating_ShouldReturnCreatedAtAction_WhenRatingIsAdded()
        {
            // Arrange
            var ratingCreateDto = new RatingCreateDTO { MoodysRating = "A", SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 };
            var newRating = new Rating { Id = 1, MoodysRating = "A", SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 };

            _mockRatingService.Setup(s => s.CreateRatingAsync(It.IsAny<Rating>())).ReturnsAsync(newRating);

            // Act
            var result = await _controller.AddRating(ratingCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Created rating with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddRating_ShouldLogWarning_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("MoodysRating", "MoodysRating is required");
            var ratingCreateDto = new RatingCreateDTO { SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 };

            // Act
            var result = await _controller.AddRating(ratingCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid model state for AddRating request")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateRating_ShouldReturnOkResult_WhenRatingIsUpdated()
        {
            // Arrange
            int ratingId = 1;
            var ratingUpdateDto = new RatingUpdateDTO { MoodysRating = "A", SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 };
            var existingRating = new Rating { Id = ratingId, MoodysRating = "B", SandPRating = "BB", FitchRating = "BBB", OrderNumber = 2 };

            _mockRatingService.Setup(s => s.GetRatingByIdAsync(ratingId)).ReturnsAsync(existingRating);
            _mockRatingService.Setup(s => s.UpdateRatingAsync(ratingId, It.IsAny<Rating>())).ReturnsAsync(existingRating);

            // Act
            var result = await _controller.UpdateRating(ratingId, ratingUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updated rating with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateRating_ShouldReturnNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            int ratingId = 1;
            var ratingUpdateDto = new RatingUpdateDTO { MoodysRating = "A", SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 };

            _mockRatingService.Setup(s => s.GetRatingByIdAsync(ratingId)).ReturnsAsync((Rating)null);

            // Act
            var result = await _controller.UpdateRating(ratingId, ratingUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Rating with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteRating_ShouldReturnNoContent_WhenRatingIsDeleted()
        {
            // Arrange
            int ratingId = 1;
            var existingRating = new Rating { Id = ratingId, MoodysRating = "A", SandPRating = "AA", FitchRating = "AAA", OrderNumber = 1 };

            _mockRatingService.Setup(s => s.GetRatingByIdAsync(ratingId)).ReturnsAsync(existingRating);
            _mockRatingService.Setup(s => s.DeleteRatingAsync(ratingId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRating(ratingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Deleted rating with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteRating_ShouldReturnNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            int ratingId = 1;
            _mockRatingService.Setup(s => s.GetRatingByIdAsync(ratingId)).ReturnsAsync((Rating)null);

            // Act
            var result = await _controller.DeleteRating(ratingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Rating with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
