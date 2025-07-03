using Gatekeeper.Models;

namespace Auth.Core
{
    public interface IAuthenticator
    {
        bool Authenticate(MessageDto message);
        bool Authorize(MessageDto message);
    }
}
