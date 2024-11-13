using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Services.Interface
{
    public interface ICurvePointService
    {
        Task<IEnumerable<CurvePoint>> GetAllCurvePointsAsync();
        Task<CurvePoint> GetCurvePointByIdAsync(int id);
        Task<CurvePoint> CreateCurvePointAsync(CurvePoint curvePoint);
        Task<CurvePoint> UpdateCurvePointAsync(int id, CurvePoint curvePoint);
        Task<bool> DeleteCurvePointAsync(int id);
    }
}
