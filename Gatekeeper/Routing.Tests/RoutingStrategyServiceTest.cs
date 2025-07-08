using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Routing.Core;
using Routing.Core.Models;
using System.Threading.Tasks;
using Xunit;

namespace Routing.Tests {
    public class RoutingStrategyServiceTests {
        [Fact]
        public async Task DetermineRouteAsync_KnownPath_ReturnsExpectedRoute() {
            // Arrange
            var mockRepo = new Mock<IRouteRepository>();
            var logger = new Mock<ILogger<RoutingStrategyService>>();

            var expectedRoute = new RouteResult {
                TargetEndpoint = "http://internal-service/api/process",
                RouteType = "Http"
            };

            mockRepo.Setup(repo => repo.ResolveTargetEndpoint("/api/GetCaseDetails"))
                    .Returns(expectedRoute);

            var strategyService = new RoutingStrategyService(mockRepo.Object, logger.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/GetCaseDetails";

            // Act
            var result = await strategyService.DetermineRouteAsync(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRoute.TargetEndpoint, result.TargetEndpoint);
            Assert.Equal(expectedRoute.RouteType, result.RouteType);
        }

        [Fact]
        public async Task DetermineRouteAsync_UnknownPath_ReturnsNull() {
            // Arrange
            var mockRepo = new Mock<IRouteRepository>();
            var logger = new Mock<ILogger<RoutingStrategyService>>();

            mockRepo.Setup(repo => repo.ResolveTargetEndpoint("/unknown")).Returns((RouteResult)null);

            var strategyService = new RoutingStrategyService(mockRepo.Object, logger.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/unknown";

            // Act
            var result = await strategyService.DetermineRouteAsync(context);

            // Assert
            Assert.Null(result);
        }
    }
}
