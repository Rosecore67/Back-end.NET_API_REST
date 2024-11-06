using Dot.Net.WebApi.Domain;
using Dot.Net.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.UserDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Fullname = u.Fullname,
                Role = u.Role
            }).ToList();

            return Ok(userDtos);
        }

        // POST: api/user/add
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDTO userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = userCreateDto.UserName,
                Email = userCreateDto.Email,
                Password = userCreateDto.Password, // Note : Dans une application réelle, assurez-vous de hacher le mot de passe
                Fullname = userCreateDto.Fullname,
                Role = userCreateDto.Role
            };

            var createdUser = await _userService.CreateUserAsync(user);

            var createdUserDto = new UserDTO
            {
                Id = createdUser.Id,
                UserName = createdUser.UserName,
                Email = createdUser.Email,
                Fullname = createdUser.Fullname,
                Role = createdUser.Role
            };

            return CreatedAtAction(nameof(GetAllUsers), new { id = createdUserDto.Id }, createdUserDto);
        }

        // PUT: api/user/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            existingUser.UserName = userUpdateDto.UserName;
            existingUser.Email = userUpdateDto.Email;
            existingUser.Fullname = userUpdateDto.Fullname;
            existingUser.Role = userUpdateDto.Role;

            var updatedUser = await _userService.UpdateUserAsync(id, existingUser);

            var updatedUserDto = new UserDTO
            {
                Id = updatedUser.Id,
                UserName = updatedUser.UserName,
                Email = updatedUser.Email,
                Fullname = updatedUser.Fullname,
                Role = updatedUser.Role
            };

            return Ok(updatedUserDto);
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }

            return NoContent();
        }

        // GET: api/user/secure/article-details
        [HttpGet("secure/article-details")]
        public async Task<ActionResult<List<UserDTO>>> GetAllUserArticles()
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Fullname = u.Fullname,
                Role = u.Role
            }).ToList();

            return Ok(userDtos);
        }
    }
}