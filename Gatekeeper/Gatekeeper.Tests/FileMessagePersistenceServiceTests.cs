using Gatekeeper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Recovery.Core;
using Recovery.Core.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Gatekeeper.Tests.Recovery {
    public class FileMessagePersistenceServiceTests :IDisposable {
        private readonly string _testDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestRecoveryFiles");
        private readonly FileMessagePersistenceService _service;
        private readonly Mock<ILogger<FileMessagePersistenceService>> _loggerMock;

        public FileMessagePersistenceServiceTests() {
            _loggerMock = new Mock<ILogger<FileMessagePersistenceService>>();
            _service = new FileMessagePersistenceService(_testDirectory, _loggerMock.Object);

            if(!Directory.Exists(_testDirectory))
                Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public async Task PersistMessageAsync_CreatesFileSuccessfully() {
            var message = new MessageWrapper {
                Message = new MessageDto {
                    SenderId = "TestSender",
                    MessageType = "TestType",
                    Payload = "TestPayload"
                },
                Timestamp = DateTime.UtcNow
            };

            var context = new DefaultHttpContext();
            context.Request.Headers["TestHeader"] = "HeaderValue";

            await _service.PersistMessageAsync(message.Message, context);

            var storedMessages = await _service.GetPendingMessagesAsync();
            Assert.Single(storedMessages);
            Assert.Equal("TestSender", storedMessages[0].wrapper.Message.SenderId);
        }

        [Fact]
        public async Task GetPendingMessagesAsync_ReturnsPersistedMessages() {
            var message = new MessageWrapper {
                Message = new MessageDto {
                    SenderId = "AnotherSender",
                    MessageType = "AnotherType",
                    Payload = "AnotherPayload"
                },
                Timestamp = DateTime.UtcNow
            };

            var context = new DefaultHttpContext();
            await _service.PersistMessageAsync(message.Message, context);

            var messages = await _service.GetPendingMessagesAsync();

            Assert.NotEmpty(messages);
            Assert.Contains(messages, m => m.wrapper.Message.SenderId == "AnotherSender");
        }

        [Fact]
        public async Task GetPendingMessagesAsync_RecognizesManualFiles() {
            var id = Guid.NewGuid().ToString();
            var filePath = Path.Combine(_testDirectory, "Recovery", $"{id}.json");
            await File.WriteAllTextAsync(filePath, "{}\n");

            var result = (await _service.GetPendingMessagesAsync()).Select(m => m.fileName).ToList();
            Assert.Contains($"{id}.json", result);
        }

        [Fact]
        public async Task DeleteMessageAsync_RemovesFileIfExists() {
            var id = Guid.NewGuid().ToString();
            var filePath = Path.Combine(_testDirectory, "Recovery", $"{id}.json");
            await File.WriteAllTextAsync(filePath, "{}");

            await _service.DeleteMessageAsync($"{id}.json");

            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public async Task LoadMessageAsync_ExistingFile_ReturnsMessageWrapper() {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var fileName = $"{id}.json";
            var filePath = Path.Combine(_testDirectory, "Recovery", fileName);

            var expected = new MessageWrapper {
                Message = new MessageDto {
                    SenderId = "LoadTestSender",
                    MessageType = "LoadTestType",
                    Payload = "LoadTestPayload"
                },
                Headers = new Dictionary<string, string> { { "TestHeader", "HeaderValue" } },
                Timestamp = DateTimeOffset.UtcNow
            };

            var json = JsonSerializer.Serialize(expected);
            await File.WriteAllTextAsync(filePath, json);

            // Act
            var result = await _service.LoadMessageAsync(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message.SenderId, result!.Message.SenderId);
            Assert.Equal(expected.Message.MessageType, result.Message.MessageType);
            Assert.Equal(expected.Message.Payload, result.Message.Payload);
            Assert.Equal("HeaderValue", result.Headers["TestHeader"]);
        }


        public void Dispose() {
            var recoveryPath = Path.Combine(_testDirectory, "Recovery");

            if(Directory.Exists(recoveryPath)) {
                foreach(var file in Directory.GetFiles(recoveryPath)) {
                    File.Delete(file);
                }
                Directory.Delete(recoveryPath, recursive: true);
            }

            if(Directory.Exists(_testDirectory)) {
                Directory.Delete(_testDirectory, recursive: true);
            }
        }
    }
}
