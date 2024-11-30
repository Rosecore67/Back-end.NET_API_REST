using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.RatingDTOs
{
    public class RatingCreateDTO
    {
        [Required(ErrorMessage = "MoodysRating is required.")]
        [StringLength(50, ErrorMessage = "MoodysRating cannot exceed 50 characters.")]
        public string MoodysRating { get; set; }

        [Required(ErrorMessage = "SandPRating is required.")]
        [StringLength(50, ErrorMessage = "SandPRating cannot exceed 50 characters.")]
        public string SandPRating { get; set; }

        [Required(ErrorMessage = "FitchRating is required.")]
        [StringLength(50, ErrorMessage = "FitchRating cannot exceed 50 characters.")]
        public string FitchRating { get; set; }

        [Range(0, 255, ErrorMessage = "OrderNumber must be between 0 and 255.")]
        public byte? OrderNumber { get; set; }
    }
}
