using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Gamification;

namespace CraftMyFit.Data.Interfaces
{
    public interface IAchievementRepository : IRepository<Achievement>
    {
        Task<List<Achievement>> GetByUserIdAsync(int userId);
        Task<List<Achievement>> GetUnlockedByUserIdAsync(int userId);
        Task<List<Achievement>> GetLockedByUserIdAsync(int userId);
        Task<List<Achievement>> GetByTypeAsync(int userId, AchievementType type);
        Task<Achievement?> GetByUserIdAndTypeAndTargetAsync(int userId, AchievementType type, int targetValue);
        Task<bool> UnlockAchievementAsync(int achievementId);
        Task<int> GetTotalPointsByUserIdAsync(int userId);
        Task<List<Achievement>> GetRecentlyUnlockedAsync(int userId, int days = 7);
        Task<bool> CheckAndUnlockAchievementsAsync(int userId, AchievementType type, int currentValue);
    }
}