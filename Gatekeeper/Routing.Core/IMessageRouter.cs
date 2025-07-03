using Gatekeeper.Models;

namespace Routing.Core
{
    public interface IMessageRouter
    {
        Task RouteAsync(MessageDto message);
    }
}
