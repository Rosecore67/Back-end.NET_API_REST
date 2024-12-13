using Dot.Net.WebApi.Controllers;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Models.DTOs.TradeDTOs;
using P7CreateRestApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class TradeControllerTests
    {
        private Mock<ITradeService> _mockTradeService;
        private Mock<ILogger<TradeController>> _mockLogger;
        private TradeController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockTradeService = new Mock<ITradeService>();
            _mockLogger = new Mock<ILogger<TradeController>>();
            _controller = new TradeController(_mockTradeService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllTrades_ShouldReturnOkResult_WithListOfTrades()
        {
            // Arrange
            var trades = new List<Trade>
    {
        new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100 },
        new Trade { TradeId = 2, Account = "Account2", AccountType = "Type2", BuyQuantity = 200 }
    };
            _mockTradeService.Setup(s => s.GetAllTradesAsync()).ReturnsAsync(trades);

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var tradeList = okResult.Value as List<TradeDTO>;
            Assert.AreEqual(2, tradeList.Count);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received request to GET all trades")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllTrades_ShouldLogWarning_WhenNoTradesFound()
        {
            // Arrange
            _mockTradeService.Setup(s => s.GetAllTradesAsync()).ReturnsAsync(new List<Trade>());

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No trades found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllTrades_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            _mockTradeService.Setup(s => s.GetAllTradesAsync())
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = result.Result as ObjectResult;

            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while retrieving all trades")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddTrade_ShouldReturnCreatedAtAction_WhenTradeIsAdded()
        {
            // Arrange
            var tradeCreateDto = new TradeCreateDTO { Account = "New Account", AccountType = "Type1", BuyQuantity = 100 };
            var newTrade = new Trade { TradeId = 1, Account = "New Account", AccountType = "Type1", BuyQuantity = 100 };

            _mockTradeService.Setup(s => s.CreateTradeAsync(It.IsAny<Trade>())).ReturnsAsync(newTrade);

            // Act
            var result = await _controller.AddTrade(tradeCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Created trade with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddTrade_ShouldLogWarning_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Account", "Account is required");
            var tradeCreateDto = new TradeCreateDTO { AccountType = "Type1", BuyQuantity = 100 };

            // Act
            var result = await _controller.AddTrade(tradeCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid model state for AddTrade request")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task AddTrade_ShouldReturnInternalError_OnException()
        {
            // Arrange
            var tradeCreateDto = new TradeCreateDTO
            {
                Account = "Account1",
                AccountType = "Type1",
                BuyQuantity = 100
            };

            _mockTradeService.Setup(s => s.CreateTradeAsync(It.IsAny<Trade>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.AddTrade(tradeCreateDto);

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
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while creating a new trade")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateTrade_ShouldReturnOkResult_WhenTradeIsUpdated()
        {
            // Arrange
            int tradeId = 1;
            var tradeUpdateDto = new TradeUpdateDTO { Account = "Updated Account", AccountType = "Type2", BuyQuantity = 150 };
            var existingTrade = new Trade { TradeId = tradeId, Account = "Old Account", AccountType = "Type1", BuyQuantity = 100 };

            _mockTradeService.Setup(s => s.GetTradeByIdAsync(tradeId)).ReturnsAsync(existingTrade);
            _mockTradeService.Setup(s => s.UpdateTradeAsync(tradeId, It.IsAny<Trade>())).ReturnsAsync(existingTrade);

            // Act
            var result = await _controller.UpdateTrade(tradeId, tradeUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updated trade with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateTrade_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            int tradeId = 1;
            var tradeUpdateDto = new TradeUpdateDTO { Account = "Updated Account", AccountType = "Type2", BuyQuantity = 150 };

            _mockTradeService.Setup(s => s.GetTradeByIdAsync(tradeId)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.UpdateTrade(tradeId, tradeUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Trade with ID {tradeId} not found for update")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateTrade_ShouldReturnInternalError_OnException()
        {
            // Arrange
            int tradeId = 1;
            var tradeUpdateDto = new TradeUpdateDTO
            {
                Account = "UpdatedAccount",
                AccountType = "UpdatedType",
                BuyQuantity = 200
            };

            _mockTradeService.Setup(s => s.GetTradeByIdAsync(tradeId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.UpdateTrade(tradeId, tradeUpdateDto);

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
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while updating the trade with ID {tradeId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteTrade_ShouldReturnNoContent_WhenTradeIsDeleted()
        {
            // Arrange
            int tradeId = 1;
            var existingTrade = new Trade { TradeId = tradeId, Account = "Account", AccountType = "Type1", BuyQuantity = 100 };

            _mockTradeService.Setup(s => s.GetTradeByIdAsync(tradeId)).ReturnsAsync(existingTrade);
            _mockTradeService.Setup(s => s.DeleteTradeAsync(tradeId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTrade(tradeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Deleted trade with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteTrade_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            int tradeId = 1;
            _mockTradeService.Setup(s => s.GetTradeByIdAsync(tradeId)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.DeleteTrade(tradeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Trade with ID {tradeId} not found for deletion")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteTrade_ShouldReturnInternalError_OnException()
        {
            // Arrange
            int tradeId = 1;

            _mockTradeService.Setup(s => s.GetTradeByIdAsync(tradeId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.DeleteTrade(tradeId);

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
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while deleting the trade with ID {tradeId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
