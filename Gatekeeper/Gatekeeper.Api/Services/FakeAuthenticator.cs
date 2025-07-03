using Auth.Core;
using Gatekeeper.Models;

namespace Gatekeeper.Api.Services
{
    public class FakeAuthenticator: IAuthenticator
    {
        public bool Authenticate(MessageDto message)
        {
            // Dummy logic: accept any message with "SenderId starting with client"
            return !string.IsNullOrWhiteSpace(message.SenderId) && message.SenderId.StartsWith("client");
        }

        public bool Authorize(MessageDto message)
        {
            // Dummy logic: only allow message type "ping" for now
            return message.MessageType == "ping";
        }
    }
}
