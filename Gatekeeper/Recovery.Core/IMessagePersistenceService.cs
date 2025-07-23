using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;

namespace Recovery.Core {
    public interface IMessagePersistenceService {
        /// <summary>
        /// Saves a message to disk for recovery, wrapped with context information.
        /// </summary>
        Task PersistMessageAsync(MessageDto message, HttpContext context);

        /// <summary>
        /// Loads all pending messages that were saved due to prior failures or shutdowns.
        /// </summary>
        Task<List<(string fileName, GatekeeperMessage wrapper)>> GetPendingMessagesAsync();

        /// <summary>
        /// Deletes a message file after successful processing or permanent failure.
        /// </summary>
        Task DeleteMessageAsync(string fileName);

        Task<GatekeeperMessage?> LoadMessageAsync(string id);

    }
}
