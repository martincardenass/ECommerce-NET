using ECommerce_NET.Data;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Repository
{
    public class NotificationRepository : INotification
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Notification>> GetUserNotifications(int userId)
        {
            return await _context.Notifications
                .Where(u => u.Receiver_Id == userId)
                .ToListAsync();
        }
    }
}
