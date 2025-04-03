using Titube.Entities;
using Titube.Interfaces;
using System.Security.Cryptography;

namespace Titube.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public static string HashPassword(string password, byte[] salt = null, int iterations = 10000)
        {
            if (salt == null)
            {
                salt = RandomNumberGenerator.GetBytes(16);
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }
        public async Task<User> CreateUserAsync(User user)
        {
            user.Password = HashPassword(user.Password);
            try
            {
                if (await _userRepository.EmailExists(user.Email) || await _userRepository.UsernameExists(user.Username))
                {
                    _logger.LogWarning("Email {Email} or Username {Username} already exists", user.Email, user.Username);
                    throw new Exception("Email already exists");
                }
                return await _userRepository.CreateUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with username {Username} and email {Email}", user.Username, user.Email);
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                var userfounded = await _userRepository.GetByIdAsync(user.Id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", user?.Id);
                    return null;
                }
                return await _userRepository.UpdateUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", user?.Id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return false;
                }
                await _userRepository.DeleteUser(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", userId);
                throw;
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }
            byte[] hashBytes = Convert.FromBase64String(storedPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            int iterations = 10000;
            using (var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }

            return true;
        }

        public async Task<User> AuthenticateAsync(string usernameOrEmail, string password)
        {
            try
            {
                User user = await _userRepository.GetByEmailAsync(usernameOrEmail);

                if (user == null || !VerifyPassword(password, user.Password))
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for {UsernameOrEmail}", usernameOrEmail);
                throw;
            }
        }

        public async Task<User> UpgradeToAdminAsync(int userId)
        {
            try
            {
                var user = await _userRepository.UpgradeToAdmin(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading user with ID {UserId} to admin", userId);
                throw;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {email} not found", email);
                    return null;
                }
                return user;
            }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error get user by email");
                    throw;
                }

        }
    }

}