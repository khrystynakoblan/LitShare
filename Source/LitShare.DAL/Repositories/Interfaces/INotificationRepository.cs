using LitShare.DAL.Models;

namespace LitShare.DAL.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notifications notification);

        Task SaveChangesAsync();

        Task<IEnumerable<Notifications>> GetUnsentNotificationsAsync();

        Task UpdateAsync(Notifications notification);
    }
}