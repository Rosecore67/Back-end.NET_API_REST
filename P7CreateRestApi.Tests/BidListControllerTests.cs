using Moq;
using P7CreateRestApi.Services.Interface;
using Dot.Net.WebApi.Controllers;
using P7CreateRestApi.Models.DTOs.BidListDTOs;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace P7CreateRestApi.Tests
{
    [TestClass]
    public class BidListControllerTests
    {
        private Mock<IBidListService> _mockBidListService;
        private Mock<ILogger<BidListController>> _mockLogger;
        private BidListController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockBidListService = new Mock<IBidListService>();
            _mockLogger = new Mock<ILogger<BidListController>>();
            _controller = new BidListController(_mockBidListService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllBids_ShouldReturnOkResult_WithListOfBids()
        {
            // Arrange
            var bids = new List<BidList>
            {
                new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" },
                new BidList { BidListId = 2, Account = "Account2", BidType = "Type2" }
            };
            _mockBidListService.Setup(s => s.GetAllBidsAsync()).ReturnsAsync(bids);

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var bidList = okResult.Value as List<BidListDTO>;
            Assert.AreEqual(2, bidList.Count);
        }

        [TestMethod]
        public async Task GetAllBids_ShouldLogWarning_WhenNoBidsFound()
        {
            // Arrange
            _mockBidListService.Setup(s => s.GetAllBidsAsync()).ReturnsAsync(new List<BidList>());

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            // Vérification du log Warning
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No bids found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllBids_ShouldReturnInternalError_OnException()
        {
            //Arrange
            var message = "Simulated exception";

            _mockBidListService.Setup(s => s.GetAllBidsAsync()).ThrowsAsync(new Exception(message));

            //Act
            var result = await _controller.GetAllBids();

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = result.Result as ObjectResult;

            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while retrieving all bids")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateBid_ValidModel_ShouldReturnCreatedResult()
        {
            // Arrange
            var bidCreateDto = new BidListCreateDTO { Account = "Account1", BidType = "Type1" };
            var newBid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            _mockBidListService.Setup(s => s.CreateBidAsync(It.IsAny<BidList>())).Returns(Task.FromResult(newBid));

            // Act
            var result = await _controller.CreateBid(bidCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task CreateBid_ShouldLogWarning_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Account", "Account is required");
            var bidCreateDto = new BidListCreateDTO
            {
                BidType = "Type1",
                BidQuantity = 10
            };

            // Act
            var result = await _controller.CreateBid(bidCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            // Vérification du log Warning
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
        public async Task CreateBid_ShouldReturnInternalError_OnException()
        {
            // Arrange
            var bidCreateDto = new BidListCreateDTO
            {
                Account = "TestAccount",
                BidType = "TestType",
                BidQuantity = 10
            };

            _mockBidListService.Setup(s => s.CreateBidAsync(It.IsAny<BidList>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.CreateBid(bidCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;

            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while creating a new bid")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateBid_ShouldReturnOkResult_WhenBidIsUpdated()
        {
            // Arrange
            int bidId = 1;
            var bidUpdateDto = new BidListUpdateDTO
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 20,
                AskQuantity = 30,
                Bid = 100.5,
                Ask = 101.5
            };

            var existingBid = new BidList
            {
                BidListId = bidId,
                Account = "OriginalAccount",
                BidType = "OriginalType",
                BidQuantity = 10,
                AskQuantity = 20,
                Bid = 99.5,
                Ask = 100.5
            };

            _mockBidListService.Setup(s => s.GetBidByIdAsync(bidId)).ReturnsAsync(existingBid);
            _mockBidListService.Setup(s => s.UpdateBidAsync(bidId, It.IsAny<BidList>())).ReturnsAsync(existingBid);

            // Act
            var result = await _controller.UpdateBid(bidId, bidUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var updatedBid = okResult.Value as BidList;
            Assert.AreEqual("UpdatedAccount", updatedBid.Account);
            Assert.AreEqual("UpdatedType", updatedBid.BidType);
            Assert.AreEqual(20, updatedBid.BidQuantity);
        }

        [TestMethod]
        public async Task UpdateBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            int bidId = 1;
            var bidUpdateDto = new BidListUpdateDTO
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 20,
                AskQuantity = 30,
                Bid = 100.5,
                Ask = 101.5
            };

            _mockBidListService.Setup(s => s.GetBidByIdAsync(bidId)).ReturnsAsync((BidList)null);

            // Act
            var result = await _controller.UpdateBid(bidId, bidUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateBid_ShouldReturnInternalError_OnException()
        {
            // Arrange
            int bidId = 1;
            var bidUpdateDto = new BidListUpdateDTO
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 20,
                AskQuantity = 30,
                Bid = 100.5,
                Ask = 101.5
            };

            var existingBid = new BidList
            {
                BidListId = bidId,
                Account = "OriginalAccount",
                BidType = "OriginalType",
                BidQuantity = 10,
                AskQuantity = 20,
                Bid = 99.5,
                Ask = 100.5
            };
            _mockBidListService.Setup(s => s.GetBidByIdAsync(bidId))
                .ReturnsAsync(existingBid);

            _mockBidListService.Setup(s => s.UpdateBidAsync(bidId, It.IsAny<BidList>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.UpdateBid(bidId, bidUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;

            Assert.AreEqual(500, objectResult.StatusCode);

            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while updating the bid with ID")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteBid_BidExists_ShouldReturnNoContent()
        {
            // Arrange
            var bidId = 1;
            _mockBidListService.Setup(s => s.GetBidByIdAsync(bidId)).ReturnsAsync(new BidList { BidListId = bidId });
            _mockBidListService.Setup(s => s.DeleteBidAsync(bidId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBid(bidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteBid_ShouldLogWarning_WhenBidNotFound()
        {
            // Arrange
            int bidId = 1;
            _mockBidListService.Setup(s => s.GetBidByIdAsync(bidId)).ReturnsAsync((BidList)null);

            // Act
            var result = await _controller.DeleteBid(bidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            // Vérification du log Warning
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Bid with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteBid_ShouldReturnInternalError_OnException()
        {
            // Arrange
            int bidId = 1;

            var existingBid = new BidList { BidListId = bidId, Account = "TestAccount" };
            _mockBidListService.Setup(s => s.GetBidByIdAsync(bidId))
                .ReturnsAsync(existingBid);

            _mockBidListService.Setup(s => s.DeleteBidAsync(bidId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.DeleteBid(bidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;

            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred. Please try again later.", objectResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while deleting the bid with ID")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}