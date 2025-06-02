using CraftMyFit.Data.Interfaces;

namespace CraftMyFit.Services
{
    public class StatsService
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IBodyMeasurementRepository _bodyMeasurementRepository;
        private readonly IProgressPhotoRepository _progressPhotoRepository;
        private readonly IAchievementRepository _achievementRepository;

        public StatsService(
            IWorkoutPlanRepository workoutPlanRepository,
            IBodyMeasurementRepository bodyMeasurementRepository,
            IProgressPhotoRepository progressPhotoRepository,
            IAchievementRepository achievementRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _bodyMeasurementRepository = bodyMeasurementRepository;
            _progressPhotoRepository = progressPhotoRepository;
            _achievementRepository = achievementRepository;
        }

        /// <summary>
        /// Calcola le statistiche complete dell'utente
        /// </summary>
        public async Task<UserStats> CalculateUserStatsAsync(int userId)
        {
            UserStats stats = new() { UserId = userId };

            try
            {
                // Statistiche allenamenti
                List<Models.Workout.WorkoutPlan> workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(userId);
                stats.TotalWorkoutPlans = workoutPlans.Count;
                stats.ActiveWorkoutPlans = workoutPlans.Count(wp => wp.ModifiedDate >= DateTime.Now.AddDays(-30));

                // Statistiche misurazioni
                List<Models.Progress.BodyMeasurement> measurements = await _bodyMeasurementRepository.GetByUserIdAsync(userId);
                stats.TotalMeasurements = measurements.Count;

                if(measurements.Any())
                {
                    Models.Progress.BodyMeasurement latest = measurements.First();
                    Models.Progress.BodyMeasurement oldest = measurements.Last();
                    stats.CurrentWeight = latest.Weight;
                    stats.WeightChange = latest.Weight - oldest.Weight;
                }

                // Statistiche foto
                List<Models.Progress.ProgressPhoto> photos = await _progressPhotoRepository.GetByUserIdAsync(userId);
                stats.TotalProgressPhotos = photos.Count;

                // Statistiche achievement
                List<Models.Gamification.Achievement> achievements = await _achievementRepository.GetByUserIdAsync(userId);
                stats.TotalAchievements = achievements.Count;
                stats.UnlockedAchievements = achievements.Count(a => a.UnlockedDate.HasValue);
                stats.TotalPoints = achievements.Where(a => a.UnlockedDate.HasValue).Sum(a => a.PointsAwarded);

                // Calcola streak
                stats.CurrentStreak = await CalculateCurrentStreakAsync(userId);
                stats.LongestStreak = await CalculateLongestStreakAsync(userId);

                // Statistiche temporali
                stats.CalculatedDate = DateTime.Now;

                return stats;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel calcolo statistiche: {ex.Message}");
                return stats;
            }
        }

        /// <summary>
        /// Calcola la streak corrente dell'utente
        /// </summary>
        private async Task<int> CalculateCurrentStreakAsync(int userId)
        {
            try
            {
                // Implementazione semplificata - in realtà dovresti controllare 
                // le sessioni di allenamento per calcolare la streak
                return await Task.FromResult(5); // Placeholder
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore calcolo streak corrente: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Calcola la streak più lunga dell'utente
        /// </summary>
        private async Task<int> CalculateLongestStreakAsync(int userId)
        {
            try
            {
                // Implementazione semplificata
                return await Task.FromResult(12); // Placeholder
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore calcolo streak più lunga: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Calcola le statistiche settimanali
        /// </summary>
        public async Task<WeeklyStats> CalculateWeeklyStatsAsync(int userId)
        {
            WeeklyStats stats = new() { UserId = userId };

            try
            {
                DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                DateTime endOfWeek = startOfWeek.AddDays(7);

                // Statistiche della settimana corrente
                List<Models.Progress.BodyMeasurement> measurements = await _bodyMeasurementRepository.GetByUserIdAndDateRangeAsync(userId, startOfWeek, endOfWeek);
                stats.MeasurementsThisWeek = measurements.Count;

                List<Models.Progress.ProgressPhoto> photos = await _progressPhotoRepository.GetByUserIdAndDateRangeAsync(userId, startOfWeek, endOfWeek);
                stats.PhotosThisWeek = photos.Count;

                stats.WeekStartDate = startOfWeek;
                stats.WeekEndDate = endOfWeek;

                return stats;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore calcolo statistiche settimanali: {ex.Message}");
                return stats;
            }
        }
    }

    // Modelli per le statistiche
    public class UserStats
    {
        public int UserId { get; set; }

        // Statistiche allenamenti
        public int TotalWorkoutPlans { get; set; }
        public int ActiveWorkoutPlans { get; set; }
        public int TotalWorkoutSessions { get; set; }
        public int CompletedWorkouts { get; set; }
        public TimeSpan TotalWorkoutTime { get; set; }

        // Statistiche progressi
        public int TotalMeasurements { get; set; }
        public int TotalProgressPhotos { get; set; }
        public float? CurrentWeight { get; set; }
        public float? WeightChange { get; set; }

        // Statistiche achievement/gamification
        public int TotalAchievements { get; set; }
        public int UnlockedAchievements { get; set; }
        public int TotalPoints { get; set; }

        // Statistiche streak
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }

        // Meta
        public DateTime CalculatedDate { get; set; }

        // Proprietà calcolate
        public float AchievementCompletionRate => TotalAchievements > 0 ? (float)UnlockedAchievements / TotalAchievements * 100 : 0;
        public string WeightChangeText => WeightChange switch
        {
            > 0 => $"+{WeightChange:F1} kg",
            < 0 => $"{WeightChange:F1} kg",
            _ => "Nessun cambiamento"
        };
    }

    public class WeeklyStats
    {
        public int UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }

        public int WorkoutsCompleted { get; set; }
        public int PlannedWorkouts { get; set; }
        public int MeasurementsThisWeek { get; set; }
        public int PhotosThisWeek { get; set; }
        public TimeSpan TotalActiveTime { get; set; }

        // Proprietà calcolate
        public float CompletionRate => PlannedWorkouts > 0 ? (float)WorkoutsCompleted / PlannedWorkouts * 100 : 0;
        public bool WeeklyGoalMet => CompletionRate >= 100;
    }
}
