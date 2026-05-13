using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LitShare.BLL.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubNotificationService notificationService;
        private readonly ILogger<NotificationBackgroundService> logger;

        public NotificationBackgroundService(
            IServiceScopeFactory scopeFactory,
            IHubNotificationService notificationService,
            ILogger<NotificationBackgroundService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.notificationService = notificationService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Notification background service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await this.ProcessNotificationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "An error occurred while processing notifications.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
        {
            using var scope = this.scopeFactory.CreateScope();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var unsentNotifications = await notificationRepo.GetUnsentNotificationsAsync();

            if (!unsentNotifications.Any())
            {
                return;
            }

            foreach (var notification in unsentNotifications)
            {
                await this.notificationService.SendToUserAsync(notification.UserId, notification.Message, stoppingToken);

                notification.IsSent = true;

                await notificationRepo.UpdateAsync(notification);
            }

            await notificationRepo.SaveChangesAsync();
        }
    }
}