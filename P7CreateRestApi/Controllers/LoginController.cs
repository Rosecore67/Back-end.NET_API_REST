using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Models.DTOs.UserDTOs;
using P7CreateRestApi.Utils;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtUtils _jwtUtils;
        private readonly ILogger<LoginController> _logger;

        public LoginController(UserManager<User> userManager, JwtUtils jwtUtils, ILogger<LoginController> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtUtils = jwtUtils;
        }


        // POST: api/Login/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO userCreateDto)
        {
            _logger.LogInformation("Received request to REGISTER a new user");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for Register request");
                    return BadRequest(ModelState);
                }

                var user = new User
                {
                    UserName = userCreateDto.UserName,
                    Email = userCreateDto.Email,
                    Fullname = userCreateDto.Fullname,
                    Role = "User"
                };

                var result = await _userManager.CreateAsync(user, userCreateDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to register user. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest("Failed to register user");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, RoleCollection.User);
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Failed to assign role to user. Errors: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return BadRequest("Failed to assign role to user");
                }

                _logger.LogInformation("User registered successfully with ID {UserId}", user.Id);

                return Ok(new { user.Id, user.UserName, user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a new user");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // POST: api/Login/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            _logger.LogInformation("Received request to LOGIN a user");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for Login request");
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByNameAsync(loginModel.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    var token = _jwtUtils.GenerateJwtToken(user);
                    _logger.LogInformation("User logged in successfully with ID {UserId}", user.Id);
                    return Ok(new { Token = token });
                }

                _logger.LogWarning("Invalid username or password for login attempt with username {Username}", loginModel.Username);
                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in a user");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}