using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LitShare.DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly LitShareDbContext context;

        public NotificationRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Notifications notification)
        {
            await this.context.Notifications.AddAsync(notification);
        }

        public async Task<IEnumerable<Notifications>> GetUnsentNotificationsAsync()
        {
            return await this.context.Notifications
                .Where(n => !n.IsSent)
                .ToListAsync();
        }

        public async Task UpdateAsync(Notifications notification)
        {
            this.context.Notifications.Update(notification);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }
    }
}