using CraftMyFit.Data.Interfaces;
using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Gamification;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Repositories
{
    public class AchievementRepository : Repository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(CraftMyFitDbContext context) : base(context)
        {
        }

        public async Task<List<Achievement>> GetByUserIdAsync(int userId) => await _context.Achievements
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.UnlockedDate)
                .ThenBy(a => a.Type)
                .ThenBy(a => a.TargetValue)
                .ToListAsync();

        public async Task<List<Achievement>> GetUnlockedByUserIdAsync(int userId) => await _context.Achievements
                .Where(a => a.UserId == userId && a.UnlockedDate.HasValue)
                .OrderByDescending(a => a.UnlockedDate)
                .ToListAsync();

        public async Task<List<Achievement>> GetLockedByUserIdAsync(int userId) => await _context.Achievements
                .Where(a => a.UserId == userId && !a.UnlockedDate.HasValue)
                .OrderBy(a => a.Type)
                .ThenBy(a => a.TargetValue)
                .ToListAsync();

        public async Task<List<Achievement>> GetByTypeAsync(int userId, AchievementType type) => await _context.Achievements
                .Where(a => a.UserId == userId && a.Type == type)
                .OrderBy(a => a.TargetValue)
                .ToListAsync();

        public async Task<Achievement?> GetByUserIdAndTypeAndTargetAsync(int userId, AchievementType type, int targetValue) => await _context.Achievements
                .FirstOrDefaultAsync(a => a.UserId == userId &&
                                        a.Type == type &&
                                        a.TargetValue == targetValue);

        public async Task<bool> UnlockAchievementAsync(int achievementId)
        {
            Achievement achievement = await GetByIdAsync(achievementId);
            if(achievement == null || achievement.UnlockedDate.HasValue)
            {
                return false;
            }

            achievement.UnlockedDate = DateTime.Now;
            await UpdateAsync(achievement);
            return true;
        }

        public async Task<int> GetTotalPointsByUserIdAsync(int userId) => await _context.Achievements
                .Where(a => a.UserId == userId && a.UnlockedDate.HasValue)
                .SumAsync(a => a.PointsAwarded);

        public async Task<List<Achievement>> GetRecentlyUnlockedAsync(int userId, int days = 7)
        {
            DateTime cutoffDate = DateTime.Now.AddDays(-days);

            return await _context.Achievements
                .Where(a => a.UserId == userId &&
                           a.UnlockedDate.HasValue &&
                           a.UnlockedDate >= cutoffDate)
                .OrderByDescending(a => a.UnlockedDate)
                .ToListAsync();
        }

        public async Task<bool> CheckAndUnlockAchievementsAsync(int userId, AchievementType type, int currentValue)
        {
            var lockedAchievements = await _context.Achievements
                .Where(a => a.UserId == userId &&
                           a.Type == type &&
                           !a.UnlockedDate.HasValue &&
                           a.TargetValue <= currentValue)
                .ToListAsync();

            if(!lockedAchievements.Any())
            {
                return false;
            }

            bool anyUnlocked = false;
            foreach(var achievement in lockedAchievements)
            {
                achievement.UnlockedDate = DateTime.Now;
                anyUnlocked = true;
            }

            if(anyUnlocked)
            {
                _ = await _context.SaveChangesAsync();
            }

            return anyUnlocked;
        }

        public override async Task<List<Achievement>> GetAllAsync() => await _context.Achievements
                .Include(a => a.User)
                .OrderByDescending(a => a.UnlockedDate)
                .ThenBy(a => a.UserId)
                .ThenBy(a => a.Type)
                .ToListAsync();

        // Metodo helper per creare achievement standard per un nuovo utente
        public async Task CreateStandardAchievementsForUserAsync(int userId)
        {
            List<Achievement> standardAchievements = new()
            {
                // Achievement per allenamenti completati
                new()
                {
                    Title = "Primo Allenamento",
                    Description = "Completa il tuo primo allenamento",
                    IconPath = "achievement_first_workout.png",
                    PointsAwarded = 10,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 1,
                    UserId = userId
                },
                new()
                {
                    Title = "Costanza Principiante",
                    Description = "Completa 5 allenamenti",
                    IconPath = "achievement_5_workouts.png",
                    PointsAwarded = 25,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 5,
                    UserId = userId
                },
                new()
                {
                    Title = "Guerriero del Fitness",
                    Description = "Completa 10 allenamenti",
                    IconPath = "achievement_10_workouts.png",
                    PointsAwarded = 50,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 10,
                    UserId = userId
                },
                new()
                {
                    Title = "Atleta Dedicato",
                    Description = "Completa 25 allenamenti",
                    IconPath = "achievement_25_workouts.png",
                    PointsAwarded = 100,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 25,
                    UserId = userId
                },

                // Achievement per giorni consecutivi
                new()
                {
                    Title = "Una Settimana",
                    Description = "Allenati per 7 giorni consecutivi",
                    IconPath = "achievement_7_days.png",
                    PointsAwarded = 100,
                    Type = AchievementType.ConsecutiveDays,
                    TargetValue = 7,
                    UserId = userId
                },
                new()
                {
                    Title = "Un Mese",
                    Description = "Allenati per 30 giorni consecutivi",
                    IconPath = "achievement_30_days.png",
                    PointsAwarded = 500,
                    Type = AchievementType.ConsecutiveDays,
                    TargetValue = 30,
                    UserId = userId
                },

                // Achievement per foto di progresso
                new()
                {
                    Title = "Prima Foto",
                    Description = "Carica la tua prima foto di progresso",
                    IconPath = "achievement_first_photo.png",
                    PointsAwarded = 15,
                    Type = AchievementType.PhotosUploaded,
                    TargetValue = 1,
                    UserId = userId
                },
                new()
                {
                    Title = "Documentare il Progresso",
                    Description = "Carica 5 foto di progresso",
                    IconPath = "achievement_5_photos.png",
                    PointsAwarded = 50,
                    Type = AchievementType.PhotosUploaded,
                    TargetValue = 5,
                    UserId = userId
                }
            };

            _context.Achievements.AddRange(standardAchievements);
            _ = await _context.SaveChangesAsync();
        }
    }
}