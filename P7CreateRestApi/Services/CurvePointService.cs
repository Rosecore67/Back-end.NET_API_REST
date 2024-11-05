using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Services.Interface;
using P7CreateRestApi.Repositories.Interface;

namespace P7CreateRestApi.Services
{
    public class CurvePointService : ICurvePointService
    {
        private readonly ICurvePointRepository _curvePointRepository;

        public CurvePointService(ICurvePointRepository curvePointRepository)
        {
            _curvePointRepository = curvePointRepository;
        }

        public async Task<IEnumerable<CurvePoint>> GetAllCurvePointsAsync()
        {
            return await _curvePointRepository.GetAllAsync();
        }

        public async Task<CurvePoint> GetCurvePointByIdAsync(int id)
        {
            return await _curvePointRepository.GetByIdAsync(id);
        }

        public async Task<CurvePoint> CreateCurvePointAsync(CurvePoint curvePoint)
        {
            await _curvePointRepository.AddAsync(curvePoint);
            return curvePoint;
        }

        public async Task<CurvePoint> UpdateCurvePointAsync(int id, CurvePoint curvePoint)
        {
            var existingCurvePoint = await _curvePointRepository.GetByIdAsync(id);
            if (existingCurvePoint == null) return null;

            existingCurvePoint.CurveId = curvePoint.CurveId;
            existingCurvePoint.AsOfDate = curvePoint.AsOfDate;
            existingCurvePoint.Term = curvePoint.Term;
            existingCurvePoint.CurvePointValue = curvePoint.CurvePointValue;

            await _curvePointRepository.UpdateAsync(existingCurvePoint);
            return existingCurvePoint;
        }

        public async Task<bool> DeleteCurvePointAsync(int id)
        {
            var curvePoint = await _curvePointRepository.GetByIdAsync(id);
            if (curvePoint == null) return false;

            await _curvePointRepository.DeleteAsync(curvePoint);
            return true;
        }
    }
}
