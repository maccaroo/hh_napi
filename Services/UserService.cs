using hh_napi.Domain;
using hh_napi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using hh_napi.Utils;

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

        public async Task<User?> GetUserByIdAsync(int id) => await _userRepository.GetByIdAsync(id);
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

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