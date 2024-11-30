using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.UserDTOs
{
    public class UserUpdateDTO
    {
        [StringLength(50, ErrorMessage = "UserName cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Fullname cannot exceed 100 characters.")]
        public string Fullname { get; set; }

        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters.")]
        public string Role { get; set; }
    }
}
