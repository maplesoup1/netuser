using Microsoft.AspNetCore.Mvc;
using Titube.Entities;
using Titube.Interfaces;
using Titube.Dtos;
using AutoMapper;



namespace Titube.Controller
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> CreateUserAsync(UserCreateDto userCreateDto)
        {
            try
            {
                var user = _mapper.Map<User>(userCreateDto);

                var createdUser = await _userService.CreateUserAsync(user);

                var resultDto = _mapper.Map<UserDto>(createdUser);

                return CreatedAtAction(nameof(GetUserById), new { userId = resultDto.Id }, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById(int userId)
        {
            try
            {   

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                _mapper.Map(userUpdateDto, user);

                var updatedUser = await _userService.UpdateUserAsync(user);

                var resultDto = _mapper.Map<UserDto>(updatedUser);

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}