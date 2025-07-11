using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Recovery.Core.Models;

namespace Recovery.Core {
    public interface IMessagePersistenceService {
        /// <summary>
        /// Saves a message to disk for recovery, wrapped with context information.
        /// </summary>
        Task PersistMessageAsync(MessageDto message, HttpContext context);

        /// <summary>
        /// Loads all pending messages that were saved due to prior failures or shutdowns.
        /// </summary>
        Task<List<(string fileName, MessageWrapper wrapper)>> GetPendingMessagesAsync();

        /// <summary>
        /// Deletes a message file after successful processing or permanent failure.
        /// </summary>
        Task DeleteMessageAsync(string fileName);

        Task<MessageWrapper?> LoadMessageAsync(string id);

    }
}
