using Microsoft.AspNetCore.Mvc;
using Recovery.Core;

namespace Gatekeeper.Api.Controllers {
    [ApiController]
    [Route("retry")]
    public class RetryController :ControllerBase {
        private readonly IMessageRetryService _retryService;

        public RetryController(IMessageRetryService retryService) {
            _retryService = retryService;
        }

        [HttpPost("all")]
        public async Task<IActionResult> RetryAll() {
            await _retryService.RetryAllPendingMessagesAsync();
            return Ok("Retry initiated.");
        }
    }
}
