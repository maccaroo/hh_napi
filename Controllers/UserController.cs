using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Services;
using Microsoft.AspNetCore.Mvc;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
        {
            if (string.IsNullOrWhiteSpace(createUserRequest.Password))
            {
                return BadRequest("Password is required");
            }

            if (string.IsNullOrWhiteSpace(createUserRequest.Email))
            {
                return BadRequest("Email is required");
            }

            if (createUserRequest.Password.Length < 8)
            {
                return BadRequest("Password must be at least 8 characters long");
            }

            var user = new User
            {
                Username = createUserRequest.Username,
                Email = createUserRequest.Email
            };
            var password = createUserRequest.Password;

            var success = await _userService.CreateUserAsync(user, password);
            return success ? CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user) : BadRequest("Could not create user.");
        }
    }
}