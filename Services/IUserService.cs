using hh_napi.Domain;

namespace hh_napi.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(User user, string password);
        Task<User?> AuthenticateAsync(string username, string password);
    }
}