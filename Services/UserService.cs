using hh_napi.Domain;
using hh_napi.Persistence.Repositories;

namespace hh_napi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int id) => await _userRepository.GetByIdAsync(id);
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

        public async Task<bool> CreateUserAsync(User user)
        {
            await _userRepository.AddAsync(user);
            return await _userRepository.SaveChangesAsync();
        }
    }
}