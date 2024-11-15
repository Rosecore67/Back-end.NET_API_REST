using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.UserDTOs
{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
