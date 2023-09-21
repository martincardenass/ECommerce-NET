using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface INotification
    {
        Task<ICollection<Notification>> GetUserNotifications(int userId);
    }
}
