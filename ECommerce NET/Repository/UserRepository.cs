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

        public async Task<(bool authenticated, string result, UserLimitedDto user)> AuthenticateUser(string username, string password)
        {
            bool? isAccountLocked = await IsAccountLocked(username);

            // Explicit check because of nullable bool
            if(isAccountLocked == true)
            {
                return (false, "Your account has been locked", null);
            }

            if (await UsernameExists(username) && !string.IsNullOrEmpty(password))
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return (false, "Something went wrong", null);
                }

                // Handle login attempts being null
                int loginAttempts = user.Login_Attempts ?? 0;

                if (BCrypt.Net.BCrypt.Verify(password, user?.Password))
                {
                    // * Reset the login attempts to 0 (if any). Account locks can only be removed by admins (later to add).
                    if (loginAttempts > 0) {
                        user.Login_Attempts = 0;
                        _ = await _context.SaveChangesAsync();
                    }

                    var userToReturn = new UserLimitedDto
                    {
                        User_Id = user.User_Id,
                        Username = user.Username,
                        Profile_Picture = user.Profile_Picture
                    };

                    return (true, $"Welcome, {user.Username}", userToReturn);
                } else
                {
                    // * Every failed login attempt adds +1 to the login attemps, if 5, account will be locked
                    loginAttempts++;
                    user.Login_Attempts = loginAttempts;
                    _ = await _context.SaveChangesAsync();
                    return (
                        false,
                        $"Wrong password. You have tried {user.Login_Attempts}/5 times", null
                        );
                }
            }
            return (false, "Something went wrong", null);
        }

        public async Task<User> GetUser(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool?> IsAccountLocked(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.Equals(username));

            // Users can attempt up to 5 times, after 5 failed attemps we block the account
            if(user.Login_Attempts.HasValue && user.Login_Attempts >= 5)
            {
                user.Lockout_Enabled = true;
                //_context.Update(user);
                _ = await _context.SaveChangesAsync();
            }

            return user.Lockout_Enabled;
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
