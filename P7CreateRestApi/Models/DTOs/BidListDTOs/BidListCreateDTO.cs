using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.BidListDTOs
{
    public class BidListCreateDTO
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

        [StringLength(100, ErrorMessage = "Benchmark must not exceed 100 characters.")]
        public string Benchmark { get; set; }

        [StringLength(200, ErrorMessage = "Commentary must not exceed 200 characters.")]
        public string Commentary { get; set; }

        [StringLength(100, ErrorMessage = "BidSecurity must not exceed 100 characters.")]
        public string BidSecurity { get; set; }

        [StringLength(50, ErrorMessage = "BidStatus must not exceed 50 characters.")]
        public string BidStatus { get; set; }

        [StringLength(50, ErrorMessage = "Trader must not exceed 50 characters.")]
        public string Trader { get; set; }

        [StringLength(50, ErrorMessage = "Book must not exceed 50 characters.")]
        public string Book { get; set; }

        [StringLength(50, ErrorMessage = "CreationName must not exceed 50 characters.")]
        public string CreationName { get; set; }

        [StringLength(50, ErrorMessage = "RevisionName must not exceed 50 characters.")]
        public string RevisionName { get; set; }

        [StringLength(50, ErrorMessage = "DealName must not exceed 50 characters.")]
        public string DealName { get; set; }

        [StringLength(50, ErrorMessage = "DealType must not exceed 50 characters.")]
        public string DealType { get; set; }

        [StringLength(50, ErrorMessage = "SourceListId must not exceed 50 characters.")]
        public string SourceListId { get; set; }

        [StringLength(50, ErrorMessage = "Side must not exceed 50 characters.")]
        public string Side { get; set; }
    }
}
