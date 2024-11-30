using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.UserDTOs
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(50, ErrorMessage = "UserName cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter and one number.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Fullname is required.")]
        [StringLength(100, ErrorMessage = "Fullname cannot exceed 100 characters.")]
        public string Fullname { get; set; }

    }
}
