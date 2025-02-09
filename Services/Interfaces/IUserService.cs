using hh_napi.Domain;
using hh_napi.Models;

namespace hh_napi.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id, string? includeRelations = null);
    Task<IEnumerable<User>> GetAllUsersAsync(PaginationParams pagination, string? includeRelations = null);
    Task<bool> CreateUserAsync(User user, string password);
    Task<User?> AuthenticateAsync(string username, string password);
}
