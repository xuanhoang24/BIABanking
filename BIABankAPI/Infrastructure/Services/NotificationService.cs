using BankingSystemAPI.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BankingSystemAPI.Infrastructure.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAllAsync()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification");
        }
    }
}
