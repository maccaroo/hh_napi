using hh_napi.Domain;
using hh_napi.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Persistence.Repositories;

public class UserCredentialsRepository : Repository<UserCredentials>, IUserCredentialsRepository
{
    private readonly AppDbContext _context;

    public UserCredentialsRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<UserCredentials?> GetByUserIdAsync(int userId)
    {
        return await _context.Set<UserCredentials>().FirstOrDefaultAsync(uc => uc.UserId == userId);
    }
}
