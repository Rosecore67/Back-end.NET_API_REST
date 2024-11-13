namespace P7CreateRestApi.Models.DTOs.CurveDTOs
{
    public class CurvePointCreateDTO
    {
        public byte? CurveId { get; set; }
        public DateTime? AsOfDate { get; set; }
        public double? Term { get; set; }
        public double? CurvePointValue { get; set; }
    }
}
