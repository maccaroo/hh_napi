using hh_napi.Domain;
using Microsoft.EntityFrameworkCore;
using hh_napi.Utils;
using hh_napi.Services.Interfaces;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Services;

public class UserService : BaseService<User>, IUserService
{

    public UserService(IUnitOfWork unitOfWork, ILogger<BaseService<User>> logger) : base(logger, unitOfWork){}

    public async Task<User?> GetUserByIdAsync(int id, string? includeRelations = null)
    {
        var query = _unitOfWork.Users.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);

        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<PagedResponse<User>> GetAllUsersAsync(PaginationParams pagination, string? includeRelations = null)
    {
        var query = _unitOfWork.Users.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);
        query = ApplySearch(query, pagination.Search);

        return await GetPagedResponseAsync(query, pagination);
    }

    public async Task<bool> CreateUserAsync(User user, string password)
    {
        var (hash, salt) = PasswordHasher.HashPassword(password);


        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userCredentials = new UserCredentials
        {
            UserId = user.Id,
            PasswordHash = hash,
            Salt = salt
        };

        await _unitOfWork.UserCredentials.AddAsync(userCredentials);
        return await _unitOfWork.UserCredentials.SaveChangesAsync();
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.AsQueryable()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        if (user == null)
        {
            return null;
        }

        var userCredentials = await _unitOfWork.UserCredentials.AsQueryable()
            .FirstOrDefaultAsync(uc => uc.UserId == user.Id);

        if (userCredentials == null || !PasswordHasher.VerifyPassword(password, userCredentials.PasswordHash, userCredentials.Salt))
        {
            return null;
        }

        return user;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.Username == username);
    }
}
