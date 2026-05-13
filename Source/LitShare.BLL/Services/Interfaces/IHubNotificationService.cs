using System.Threading;
using System.Threading.Tasks;

namespace LitShare.BLL.Services.Interfaces
{
    public interface IHubNotificationService
    {
        Task SendToUserAsync(int userId, string message, CancellationToken cancellationToken = default);
    }
}