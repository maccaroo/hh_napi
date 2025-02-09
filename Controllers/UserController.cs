using AutoMapper;
using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Services;
using hh_napi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id, [FromQuery] string? includeRelations = null)
        {
            var user = await _userService.GetUserByIdAsync(id, includeRelations);
            return user != null ? Ok(_mapper.Map<UserResponse>(user)) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams pagination, [FromQuery] string? includeRelations = null)
        {
            var pagedUsers = await _userService.GetAllUsersAsync(pagination, includeRelations);
            return Ok(pagedUsers.ConvertTo<UserResponse>(_mapper));
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