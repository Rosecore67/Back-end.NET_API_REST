using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.TradeDTOs
{
    public class TradeCreateDTO
    {
        [Required(ErrorMessage = "Account is required.")]
        [StringLength(50, ErrorMessage = "Account cannot exceed 50 characters.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "AccountType is required.")]
        [StringLength(50, ErrorMessage = "AccountType cannot exceed 50 characters.")]
        public string AccountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BuyQuantity must be a positive number.")]
        public double? BuyQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SellQuantity must be a positive number.")]
        public double? SellQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BuyPrice must be a positive number.")]
        public double? BuyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SellPrice must be a positive number.")]
        public double? SellPrice { get; set; }

        public DateTime? TradeDate { get; set; }

        [Required(ErrorMessage = "TradeSecurity is required.")]
        [StringLength(100, ErrorMessage = "TradeSecurity cannot exceed 100 characters.")]
        public string TradeSecurity { get; set; }

        [Required(ErrorMessage = "TradeStatus is required.")]
        [StringLength(50, ErrorMessage = "TradeStatus cannot exceed 50 characters.")]
        public string TradeStatus { get; set; }

        [StringLength(50, ErrorMessage = "Trader cannot exceed 50 characters.")]
        public string Trader { get; set; }

        [StringLength(100, ErrorMessage = "Benchmark cannot exceed 100 characters.")]
        public string Benchmark { get; set; }

        [StringLength(50, ErrorMessage = "Book cannot exceed 50 characters.")]
        public string Book { get; set; }

        [StringLength(50, ErrorMessage = "CreationName cannot exceed 50 characters.")]
        public string CreationName { get; set; }

        [StringLength(50, ErrorMessage = "RevisionName cannot exceed 50 characters.")]
        public string RevisionName { get; set; }

        [StringLength(50, ErrorMessage = "DealName cannot exceed 50 characters.")]
        public string DealName { get; set; }

        [StringLength(50, ErrorMessage = "DealType cannot exceed 50 characters.")]
        public string DealType { get; set; }

        [StringLength(50, ErrorMessage = "SourceListId cannot exceed 50 characters.")]
        public string SourceListId { get; set; }

        [StringLength(50, ErrorMessage = "Side cannot exceed 50 characters.")]
        public string Side { get; set; }
    }
}
