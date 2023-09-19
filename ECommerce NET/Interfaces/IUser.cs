using ECommerce_NET.Dto;
using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface IUser
    {
        Task<bool> RegisterUser(User user, IFormFile file);
        Task<bool> UpdateUser(int userId, UserUpdateDto user, IFormFile file);
        Task<bool> AuthenticateUser(string username, string password);
        Task<User> GetUser(string username);
        Task<bool> UsernameExists(string username);
        Task<bool> IsEmailRegistered(string email);
    }
}
