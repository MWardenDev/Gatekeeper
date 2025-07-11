using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Recovery.Core.Models;
using System.Text.Json;

namespace Recovery.Core {
    public class FileMessagePersistenceService :IMessagePersistenceService {
        private readonly string _baseDirectory;
        private readonly ILogger<FileMessagePersistenceService> _logger;

        public FileMessagePersistenceService(string rootPath, ILogger<FileMessagePersistenceService> logger) {
            _logger = logger;
            _baseDirectory = Path.Combine(rootPath, "Recovery");

            if(!Directory.Exists(_baseDirectory))
                Directory.CreateDirectory(_baseDirectory);
        }

        public async Task<MessageWrapper?> LoadMessageAsync(string fileName) {
            var fullPath = Path.Combine(_baseDirectory, fileName);
            if(!File.Exists(fullPath)) {
                _logger.LogWarning("Load failed: File not found for {FileName}", fileName);
                return null;
            }

            try {
                var json = await File.ReadAllTextAsync(fullPath);
                var wrapper = JsonSerializer.Deserialize<MessageWrapper>(json);
                return wrapper;
            } catch(Exception ex) {
                _logger.LogError(ex, "Failed to load and deserialize message from {FileName}", fileName);
                return null;
            }
        }


        public async Task PersistMessageAsync(MessageDto message, HttpContext context) {
            var id = Guid.NewGuid().ToString("N");
            var filePath = Path.Combine(_baseDirectory, $"{id}.json");

            var wrapper = new MessageWrapper {
                Message = message,
                Headers = context?.Request?.Headers?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()) ?? new Dictionary<string, string>(),
                Timestamp = DateTimeOffset.UtcNow
            };

            var json = JsonSerializer.Serialize(wrapper);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<(string fileName, MessageWrapper wrapper)>> GetPendingMessagesAsync() {
            var files = Directory.GetFiles(_baseDirectory, "*.json");
            var messages = new List<(string, MessageWrapper)>();

            foreach(var file in files) {
                var json = await File.ReadAllTextAsync(file);
                var wrapper = JsonSerializer.Deserialize<MessageWrapper>(json);
                if(wrapper != null) {
                    messages.Add((Path.GetFileName(file), wrapper));
                }
            }

            return messages;
        }

        public Task DeleteMessageAsync(string fileName) {
            var path = Path.Combine(_baseDirectory, fileName);
            if(File.Exists(path)) {
                File.Delete(path);
            }
            return Task.CompletedTask;
        }
    }
}
