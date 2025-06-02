using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Progress;

namespace CraftMyFit.Data.Interfaces
{
    public interface IBodyMeasurementRepository : IRepository<BodyMeasurement>
    {
        Task<List<BodyMeasurement>> GetByUserIdAsync(int userId);
        Task<List<BodyMeasurement>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<BodyMeasurement?> GetLatestByUserIdAsync(int userId);
        Task<BodyMeasurement?> GetByUserIdAndDateAsync(int userId, DateTime date);
        Task<List<BodyMeasurement>> GetWeightHistoryAsync(int userId, int months = 12);
        Task<Dictionary<string, List<BodyMeasurement>>> GetMeasurementHistoryAsync(int userId, int months = 12);
        Task<bool> HasMeasurementForDateAsync(int userId, DateTime date);
        Task<float?> GetWeightChangeAsync(int userId, int days = 30);
    }
}