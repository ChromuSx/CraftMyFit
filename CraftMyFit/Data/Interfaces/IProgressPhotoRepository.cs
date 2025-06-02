using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Progress;

namespace CraftMyFit.Data.Interfaces
{
    public interface IProgressPhotoRepository : IRepository<ProgressPhoto>
    {
        Task<List<ProgressPhoto>> GetByUserIdAsync(int userId);
        Task<List<ProgressPhoto>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<ProgressPhoto?> GetLatestByUserIdAsync(int userId);
        Task<List<ProgressPhoto>> GetRecentPhotosAsync(int userId, int count = 10);
        Task<int> GetPhotoCountByUserIdAsync(int userId);
        Task<bool> HasPhotosForDateAsync(int userId, DateTime date);
        Task<List<ProgressPhoto>> GetPhotosForComparisonAsync(int userId, DateTime compareDate, int daysTolerance = 7);
    }
}