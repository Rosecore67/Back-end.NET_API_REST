namespace P7CreateRestApi.Models.DTOs
{
    public class BidListDTO
    {
        public int BidListId { get; set; }
        public string Account { get; set; }
        public string BidType { get; set; }
        public double? BidQuantity { get; set; }
        public double? AskQuantity { get; set; }
        public double? Bid { get; set; }
        public double? Ask { get; set; }
        public string Benchmark { get; set; }
        public string Commentary { get; set; }
        public string BidStatus { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
