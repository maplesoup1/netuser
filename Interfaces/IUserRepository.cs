using Titube.Entities;

namespace Titube.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(User user);

        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> UpgradeToAdmin(int userId);
        Task<bool> EmailExists(string email);
        Task<bool> UsernameExists(string username);
    }
}