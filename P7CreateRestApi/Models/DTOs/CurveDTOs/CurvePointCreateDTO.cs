using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.CurveDTOs
{
    public class CurvePointCreateDTO
    {
        [Required(ErrorMessage = "CurveId is required.")]
        [Range(0, 255, ErrorMessage = "CurveId must be between 0 and 255.")]
        public byte? CurveId { get; set; }

        [Required(ErrorMessage = "AsOfDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "AsOfDate must be a valid date.")]
        public DateTime? AsOfDate { get; set; }

        [Required(ErrorMessage = "Term is required.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Term must be a positive number.")]
        public double? Term { get; set; }

        [Required(ErrorMessage = "CurvePointValue is required.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "CurvePointValue must be a positive number.")]
        public double? CurvePointValue { get; set; }
    }
}
