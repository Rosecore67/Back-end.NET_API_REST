using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.RatingDTOs
{
    public class RatingUpdateDTO
    {
        [StringLength(50, ErrorMessage = "MoodysRating cannot exceed 50 characters.")]
        public string MoodysRating { get; set; }

        [StringLength(50, ErrorMessage = "SandPRating cannot exceed 50 characters.")]
        public string SandPRating { get; set; }

        [StringLength(50, ErrorMessage = "FitchRating cannot exceed 50 characters.")]
        public string FitchRating { get; set; }

        [Range(0, 255, ErrorMessage = "OrderNumber must be between 0 and 255.")]
        public byte? OrderNumber { get; set; }
    }
}
