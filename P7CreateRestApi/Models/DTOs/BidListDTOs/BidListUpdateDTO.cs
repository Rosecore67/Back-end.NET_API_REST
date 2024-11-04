namespace P7CreateRestApi.Models.DTOs.BidListDTOs
{
    public class BidListUpdateDTO
    {
        public string Account { get; set; }
        public string BidType { get; set; }
        public double? BidQuantity { get; set; }
        public double? AskQuantity { get; set; }
        public double? Bid { get; set; }
        public double? Ask { get; set; }
    }
}
