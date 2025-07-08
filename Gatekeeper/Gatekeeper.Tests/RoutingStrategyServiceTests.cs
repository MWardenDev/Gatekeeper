using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Routing.Core;
using Routing.Core.Models;
using Xunit;

namespace Routing.Tests {
    public class RoutingStrategyServiceTests {
        [Fact]
        public async Task DetermineRouteAsync_KnownPath_ReturnsExpectedRoute() {
            // Arrange
            var mockRepo = new Mock<IRouteRepository>();
            var logger = new Mock<ILogger<RoutingStrategyService>>();

            var expectedRoute = new RouteDefinition {
                TargetAddress = "http://internal-service/api/process",
                TargetType = "Http"
            };

            mockRepo.Setup(repo => repo.GetRouteForEndpointAsync("api"))
                    .ReturnsAsync(expectedRoute);

            var strategyService = new RoutingStrategyService(logger.Object, mockRepo.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/GetCaseDetails";

            // Act
            var result = await strategyService.DetermineRouteAsync(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRoute.TargetAddress, result.TargetAddress);
            Assert.Equal(expectedRoute.TargetType, result.TargetType);
        }

        [Fact]
        public async Task DetermineRouteAsync_UnknownPath_ThrowsException() {
            // Arrange
            var mockRepo = new Mock<IRouteRepository>();
            var logger = new Mock<ILogger<RoutingStrategyService>>();

            mockRepo.Setup(repo => repo.GetRouteForEndpointAsync("unknown"))
                    .ReturnsAsync((RouteDefinition)null);

            var strategyService = new RoutingStrategyService(logger.Object, mockRepo.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/unknown";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                strategyService.DetermineRouteAsync(context));
        }
    }
}
