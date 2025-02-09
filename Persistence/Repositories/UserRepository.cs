using hh_napi.Domain;
using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}
