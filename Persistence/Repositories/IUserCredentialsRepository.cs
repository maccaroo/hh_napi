using hh_napi.Domain;

namespace hh_napi.Persistence.Repositories
{
    public interface IUserCredentialsRepository : IRepository<UserCredentials>
    {
        Task<UserCredentials?> GetByUserIdAsync(int userId);
    }
}