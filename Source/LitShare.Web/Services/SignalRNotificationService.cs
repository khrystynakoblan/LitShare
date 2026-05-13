using System.Threading;
using System.Threading.Tasks;
using LitShare.BLL.Services.Interfaces;
using LitShare.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LitShare.Web.Services
{
    public class SignalRNotificationService : IHubNotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task SendToUserAsync(int userId, string message, CancellationToken cancellationToken)
        {
            await this.hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", message, cancellationToken: cancellationToken);
        }
    }
}