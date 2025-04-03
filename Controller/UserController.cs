using Microsoft.AspNetCore.Mvc;
using Titube.Entities;
using Titube.Interfaces;
using Titube.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Titube.Helper;
using Titube.Services;



namespace Titube.Controller
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IVerificationService _verificationService;

        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper, IConfiguration config, IVerificationService verificationService)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _verificationService = verificationService;
        }

        [Authorize]
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
                var isCodeValid = await _verificationService.VerifyCode(userCreateDto.Email, userCreateDto.VerificationCode);
                if (!isCodeValid)
                {
                    return BadRequest(new { message = "Invalid or expired verification code." });
                }

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

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            var result = await _verificationService.SendVerificationCode(request.Email);

            if (result)
            {
                return Ok(new { message = "Verification code sent to email." });
            }

            return StatusCode(500, new { message = "Failed to send verification code." });
        }

        [Authorize]
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

        [Authorize]
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


        [HttpPost("login")]
        public async Task<ActionResult<UserAuthenticationDto>> UserLogin(UserAuthenticationDto loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }
                var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }
                var token = JwtHelper.GenerateToken(loginDto.Email, _config["JwtSettings:Secret"]);
                var userDto = new UserDto
                {   
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                };

                var response = new UserLoginResponseDto
                {
                    Token = token,
                    User = userDto
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginDto.Email);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}