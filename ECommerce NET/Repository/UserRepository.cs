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
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.Equals(username));

                return (
                    false,
                    $"Access to your account is currently restricted until {user.Locked_Until}. You can try again after this time.",
                    null);
            }

            if (await UsernameExists(username) && !string.IsNullOrEmpty(password))
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.Equals(username));

                if (user is null)
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
                .FirstOrDefaultAsync(u => u.Username.Equals(username));
        }

        public async Task<bool?> IsAccountLocked(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.Equals(username));

            if(user is null)
            {
                return null;
            }

            if(user.Login_Attempts >= 5)
            {
                user.Lockout_Enabled = true;
                user.Login_Attempts = 0;
                user.Locked_Until = DateTime.UtcNow.AddMinutes(5); //  Add acount lock for 5 minutes

                // Send a notification to the user letting them know someone tried to access their account
                var notification = new Notification
                {
                    Notification_Type = "System",
                    Notification_Value = "Your account was temporary blocked because of multiple failed login attempts.",
                    Receiver_Id = user.User_Id,
                    Created = DateTime.UtcNow
                };

                _context.Add(notification);

                _ = await _context.SaveChangesAsync();
            }

            if(user.Lockout_Enabled == true)
            {
                // less than zero: 1st earlier than 2nd
                // greater than zero: 1st later than 2nd
                int dateComparasion = DateTimeOffset.Compare(DateTimeOffset.UtcNow, user.Locked_Until.Value);

                if(dateComparasion > 0)
                {
                    // Account lock already passed
                    user.Locked_Until = null;
                    user.Lockout_Enabled = false;
                    user.Login_Attempts = 0;
                    _ = await _context.SaveChangesAsync();
                }
            }

            return user.Lockout_Enabled;
        }

        public async Task<bool> IsEmailRegistered(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Equals(email));
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

            if(userToModify is not null)
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
            return await _context.Users.AnyAsync(u => u.Username.Equals(username));
        }
    }
}
