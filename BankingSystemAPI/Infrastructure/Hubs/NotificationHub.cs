using Microsoft.AspNetCore.SignalR;

namespace BankingSystemAPI.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
