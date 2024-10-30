using System;

namespace Dot.Net.WebApi.Domain
{
    public class CurvePoint
    {
        // TODO: Map columns in data table CURVEPOINT with corresponding fields
       public int Id {  get; set; }
       public byte? CurveId { get; set; }
       public DateTime? AsOfDate { get; set; }
       public double? Term {  get; set; }
       public double? CurvePointValue { get; set; }
       public DateTime? CreationDate { get; set; }
    }
}