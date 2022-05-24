using Common.DataTransferObject;
using Common.Enumerations;
using Microsoft.AspNetCore.SignalR;
using Application2.Controllers;
using Application2.Models;
using Common.Structs;

namespace Application2.Extensions
{
    public static class MainHubExtensions
    {
        private static async Task NotifiyUserActivity(this MainHub hub, User user, UserActivityType userActivityType)
        {
            if (hub is null) throw new ArgumentNullException(nameof(hub));
            if (user is null) throw new ArgumentNullException(nameof(user));

            var userActivity = new UserActivityDto {
                Type = userActivityType,
                User = user.ToDto()
            };

            await hub.Clients.All.SendAsync(RemoteProcedureCall.RECEIVE_USER_ACTIVITY, userActivity)
                .ConfigureAwait(false);

        }

        public static Task NotifiyUserConnected(this MainHub hub, User user) =>
            hub.NotifiyUserActivity(user, UserActivityType.Connection);

        public static Task NotifiyUserDisconnected(this MainHub hub, User user) =>
            hub.NotifiyUserActivity(user, UserActivityType.Disconnection);
    }
}
