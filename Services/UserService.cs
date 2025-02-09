using hh_napi.Domain;
using hh_napi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using hh_napi.Utils;
using hh_napi.Services.Interfaces;
using hh_napi.Models;

namespace hh_napi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserCredentialsRepository _userCredentialsRepository;
        
        public UserService(IUserRepository userRepository, IUserCredentialsRepository userCredentialsRepository)
        {
            _userRepository = userRepository;
            _userCredentialsRepository = userCredentialsRepository;
        }

        public async Task<User?> GetUserByIdAsync(int id, string? includeRelations = null)
        {
            var query = _userRepository.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(includeRelations))
            {
                var relations = includeRelations.Split(',');
                foreach (var relation in relations)
                {
                    query = query.Include(relation.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync(PaginationParams pagination, string? includeRelations = null)
        {
            var query = _userRepository.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(u => u.Username.Contains(pagination.Search));
            }

            if (!string.IsNullOrEmpty(includeRelations))
            {
                var relations = includeRelations.Split(',');
                foreach (var relation in relations)
                {
                    query = query.Include(relation.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var (hash, salt) = PasswordHasher.HashPassword(password);


            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var userCredentials = new UserCredentials
            {
                UserId = user.Id,
                PasswordHash = hash,
                Salt = salt
            };

            await _userCredentialsRepository.AddAsync(userCredentials);
            return await _userCredentialsRepository.SaveChangesAsync();
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.AsQueryable()
               .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (user == null)
            {
                return null;
            }

            var userCredentials = await _userCredentialsRepository.AsQueryable()
                .FirstOrDefaultAsync(uc => uc.UserId == user.Id);

            if (userCredentials == null || !PasswordHasher.VerifyPassword(password, userCredentials.PasswordHash, userCredentials.Salt))
            {
                return null;
            }

            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.AsQueryable().FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}