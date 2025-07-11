using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Recovery.Core {
    public interface IMessageRetryService {
        Task RetryMessageAsync(string fileName);
        Task RetryAllPendingMessagesAsync();
    }
}
