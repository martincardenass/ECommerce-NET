using ECommerce_NET.Data;
using ECommerce_NET.Dto;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Repository
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly IImage _imageService;

        public UserRepository(ApplicationDbContext context, IImage imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            if(await UsernameExists(username) && !string.IsNullOrEmpty(password))
            {
                string? hashedPasswordFromDatabse = await _context.Users
                    .Where(u => u.Username == username)
                    .Select(u => u.Password)
                    .FirstOrDefaultAsync();
                    
                if(BCrypt.Net.BCrypt.Verify(password, hashedPasswordFromDatabse))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<User> GetUser(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailRegistered(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> RegisterUser(User user, IFormFile file)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

            // * 300 * 300 its the profile picture size
            string? profilePictureUrl = await _imageService.UploadToCloudinary(file, 300, 300);

            var newUser = new User()
            {
                Username = user.Username.ToLower(),
                Role = user.Role.ToLower(),
                First_Name = user.First_Name,
                Last_Name = user.Last_Name,
                Password = hashedPassword,
                Gender = user.Gender,
                Profile_Picture = profilePictureUrl,
                Last_Login = user.Last_Login,
                Email = user.Email,
                Created = DateTime.UtcNow
            };

            _context.Add(newUser);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUser(int userId, UserUpdateDto user, IFormFile file)
        {
            string? hashedPassword = string.Empty;
            if(!string.IsNullOrEmpty(user.Password))
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
            }

            string? newProfilePictureUrl = await _imageService.UploadToCloudinary(file, 300, 300);

            var userToModify = await _context.Users
                .FindAsync(userId);

            if(userToModify != null)
            {
                userToModify.First_Name = user.First_Name ?? userToModify.First_Name;
                userToModify.Last_Name = user.Last_Name ?? userToModify.Last_Name;
                userToModify.Password = hashedPassword ?? userToModify.Password;
                userToModify.Gender = user.Gender ?? userToModify.Gender;
                userToModify.Profile_Picture = newProfilePictureUrl ?? userToModify.Profile_Picture;
                userToModify.Email = user.Email ?? userToModify.Email;

                _context.Update(userToModify);

                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> UsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}
