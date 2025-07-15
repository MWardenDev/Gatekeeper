using Gatekeeper.Models;
using Logging.Core;
using Microsoft.AspNetCore.Http;
using Moq;
using Recovery.Core;
using Recovery.Core.Models;
using Routing.Core;
using Routing.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Gatekeeper.Tests.Recovery {
    public class MessageRetryServiceTests {
        private readonly Mock<IMessagePersistenceService> _mockPersistence;
        private readonly Mock<IRoutingStrategyService> _mockStrategy;
        private readonly Mock<IRouteExecutorService> _mockExecutor;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly MessageRetryService _retryService;

        public MessageRetryServiceTests() {
            _mockPersistence = new Mock<IMessagePersistenceService>();
            _mockStrategy = new Mock<IRoutingStrategyService>();
            _mockExecutor = new Mock<IRouteExecutorService>();
            _mockLogger = new Mock<ILoggerService>();

            _retryService = new MessageRetryService(
                _mockPersistence.Object,
                _mockStrategy.Object,
                _mockExecutor.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task RetryAllMessagesAsync_ExecutesAndDeletesSuccessfulMessages() {
            // Arrange
            var message = new MessageWrapper {
                Message = new MessageDto {
                    SenderId = "sender",
                    MessageType = "type",
                    Payload = "data"
                },
                Timestamp = DateTimeOffset.UtcNow
            };

            _mockPersistence.Setup(p => p.GetPendingMessagesAsync())
                .ReturnsAsync(new List<(string, MessageWrapper)> {
            ("test.json", message)
                });

            _mockPersistence.Setup(p => p.LoadMessageAsync("test.json"))
                .ReturnsAsync(message);

            _mockStrategy.Setup(s => s.DetermineRouteAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new RouteDefinition {
                    TargetAddress = "http://dummy",
                    TargetType = "Http"
                });

            _mockExecutor.Setup(e => e.ExecuteRouteAsync(
                    It.IsAny<RouteDefinition>(),
                    message.Message,
                    It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask);

            // Act
            await _retryService.RetryAllPendingMessagesAsync();

            // Assert
            _mockExecutor.Verify(e => e.ExecuteRouteAsync(
                It.IsAny<RouteDefinition>(),
                message.Message,
                It.IsAny<HttpContext>()),
                Times.Once);
        }


        [Fact]
        public async Task RetryAllMessagesAsync_LogsError_WhenExecutionFails() {
            // Arrange
            var fileName = "badmsg.json";
            var wrapper = new MessageWrapper {
                Message = new MessageDto {
                    SenderId = "fail-sender",
                    MessageType = "fail-type",
                    Payload = "broken"
                },
                Headers = new Dictionary<string, string>(),
                Timestamp = DateTimeOffset.UtcNow
            };

            var pending = new List<(string, MessageWrapper)>
            {
                (fileName, wrapper)
            };

            _mockPersistence.Setup(p => p.GetPendingMessagesAsync())
                .ReturnsAsync(pending);

            _mockStrategy.Setup(s => s.DetermineRouteAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new RouteDefinition {
                    TargetType = "Http",
                    TargetAddress = "https://example.com"
                });

            _mockExecutor.Setup(e => e.ExecuteRouteAsync(It.IsAny<RouteDefinition>(), wrapper.Message, It.IsAny<HttpContext>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _retryService.RetryAllPendingMessagesAsync();

            // Assert
            _mockLogger.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Retry failed"))), Times.Once);
            _mockPersistence.Verify(p => p.DeleteMessageAsync(It.IsAny<string>()), Times.Never); // message should not be deleted
        }
    }
}
