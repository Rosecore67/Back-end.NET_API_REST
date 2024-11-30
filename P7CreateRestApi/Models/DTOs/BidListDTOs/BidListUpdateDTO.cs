using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.BidListDTOs
{
    public class BidListUpdateDTO
    {
        [Required(ErrorMessage = "Account is required.")]
        [StringLength(50, ErrorMessage = "Account must not exceed 50 characters.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "BidType is required.")]
        [StringLength(50, ErrorMessage = "BidType must not exceed 50 characters.")]
        public string BidType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BidQuantity must be a positive number.")]
        public double? BidQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "AskQuantity must be a positive number.")]
        public double? AskQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bid must be a positive number.")]
        public double? Bid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ask must be a positive number.")]
        public double? Ask { get; set; }
    }
}
