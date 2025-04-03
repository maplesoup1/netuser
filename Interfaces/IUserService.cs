using Titube.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Titube.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByEmail(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        
        Task<User> AuthenticateAsync(string usernameOrEmail, string password);
        Task<User> UpgradeToAdminAsync(int userId);
    }
}