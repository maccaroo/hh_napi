using hh_napi.Domain;
using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Persistence.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }
}
