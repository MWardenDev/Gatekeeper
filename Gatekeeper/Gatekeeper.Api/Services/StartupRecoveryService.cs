using Microsoft.Extensions.Hosting;
using Recovery.Core;
using System.Threading;
using System.Threading.Tasks;
using Logging.Core;

namespace Gatekeeper.Api.Services {
    public class StartupRecoveryHostedService :IHostedService {
        private readonly IMessageRetryService _retryService;
        private readonly ILoggerService _logger;

        public StartupRecoveryHostedService(IMessageRetryService retryService, ILoggerService logger) {
            _retryService = retryService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInfo("StartupRecoveryHostedService triggered. Checking for pending messages...");
            await _retryService.RetryAllPendingMessagesAsync();
            _logger.LogInfo("StartupRecoveryHostedService completed.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
