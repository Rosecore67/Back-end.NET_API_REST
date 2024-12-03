using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Models.DTOs.UserDTOs;


namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/user/list
        [Authorize(Roles = "Admin")]
        [HttpGet("list")]
        public IActionResult GetAllUsers()
        {
            _logger.LogInformation("Received request to GET all users");

            try
            {
                var users = _userManager.Users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Fullname = u.Fullname,
                    Role = u.Role
                }).ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // POST: api/user/add
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new user");

            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = new User
                {
                    UserName = userCreateDto.UserName,
                    Email = userCreateDto.Email,
                    Fullname = userCreateDto.Fullname,
                };

                var result = await _userManager.CreateAsync(user, userCreateDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to createe user. Errors : {Errors}", 
                        string.Join(",", result.Errors.Select(e => e.Description)));

                    return BadRequest("Failed to create user. Please check your input");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, RoleCollection.User);
                if (!roleResult.Succeeded) 
                {
                    _logger.LogWarning("Failed to assign role to user. Error: {Erros}", 
                        string.Join(",", roleResult.Errors.Select(e => e.Description)));

                    return BadRequest(roleResult.Errors); 
                }

                return Ok(new { user.Id, user.UserName, user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // PUT: api/user/update/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE user with ID {UserId}", id);

            try
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", id);
                    return NotFound();
                }

                existingUser.UserName = userUpdateDto.UserName ?? existingUser.UserName;
                existingUser.Email = userUpdateDto.Email ?? existingUser.Email;
                existingUser.Fullname = userUpdateDto.Fullname ?? existingUser.Fullname;
                existingUser.Role = userUpdateDto.Role ?? existingUser.Role;

                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to update user with ID {UserId}", id);
                    return StatusCode(500, "An error occurred while updating the user.");
                }

                _logger.LogInformation("Updated user with ID {UserId}", id);

                return Ok(new { existingUser.Id, existingUser.UserName, existingUser.Email, existingUser.Fullname, existingUser.Role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // DELETE: api/user/delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation("Received request to DELETE user with ID {UserId}", id);

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                    return NotFound();
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to delete user with ID {UserId}", id);
                    return StatusCode(500, "An error occurred while deleting the user.");
                }

                _logger.LogInformation("Deleted user with ID {UserId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID {UserId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}