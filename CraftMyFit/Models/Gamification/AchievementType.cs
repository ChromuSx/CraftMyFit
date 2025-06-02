// AchievementType.cs - Tipi di achievement completi
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models.Gamification
{
    /// <summary>
    /// Tipi di achievement disponibili nell'app
    /// </summary>
    public enum AchievementType
    {
        // Achievement legati agli allenamenti
        WorkoutsCompleted,      // Numero totale di allenamenti completati  
        ConsecutiveDays,        // Giorni consecutivi di allenamento
        WorkoutStreak,          // Streak più lunga di allenamenti
        TotalWorkoutTime,       // Tempo totale di allenamento in minuti
        WorkoutFrequency,       // Frequenza di allenamenti settimanali

        // Achievement legati ai progressi fisici
        WeightLost,             // Peso perso in kg
        WeightGained,           // Peso guadagnato in kg (massa muscolare)
        BodyFatReduced,         // Percentuale di grasso corporeo ridotta
        MuscleGained,           // Massa muscolare guadagnata
        MeasurementReduced,     // Riduzione circonferenze corporee

        // Achievement legati alle foto di progresso
        PhotosUploaded,         // Numero di foto di progresso caricate
        PhotoComparison,        // Confronti fotografici completati
        TransformationShared,   // Trasformazioni condivise

        // Achievement legati alla costanza
        WeeklyGoalMet,          // Obiettivi settimanali raggiunti
        MonthlyGoalMet,         // Obiettivi mensili raggiunti
        YearlyGoalMet,          // Obiettivi annuali raggiunti
        PerfectWeek,            // Settimana perfetta (tutti gli allenamenti)
        PerfectMonth,           // Mese perfetto

        // Achievement legati agli esercizi specifici
        PushUpsMaster,          // Padronanza nelle flessioni
        SquatChampion,          // Campione degli squat
        PlankWarrior,           // Guerriero del plank
        CardioKing,             // Re del cardio
        StrengthGuru,           // Guru della forza

        // Achievement sociali e condivisione
        FirstShare,             // Prima condivisione
        SocialInfluencer,       // Influencer social (condivisioni multiple)
        CommunityMember,        // Membro attivo della community
        Motivator,              // Motivatore (aiuta altri utenti)

        // Achievement speciali e milestone
        FirstWorkout,           // Primo allenamento completato
        FirstMonth,             // Primo mese di utilizzo app
        FirstYear,              // Primo anno di utilizzo app
        CustomGoalReached,      // Obiettivo personalizzato raggiunto
        PersonalRecord,         // Record personale battuto

        // Achievement stagionali/eventi
        NewYearChallenge,       // Sfida di Capodanno
        SummerBody,             // Corpo estivo
        HalloweenHorror,        // Sfida Halloween
        ChristmasChallenge,     // Sfida natalizia
        SpringCleaning,         // Pulizie di primavera

        // Achievement tecnici/app
        ProfileCompleted,       // Profilo completato al 100%
        AllFeatures,            // Utilizzate tutte le funzionalità
        DataBackup,             // Backup dati completato
        WearableConnected,      // Dispositivo wearable connesso
        AppRated              // App valutata nello store
    }

    /// <summary>
    /// Estensioni per AchievementType
    /// </summary>
    public static class AchievementTypeExtensions
    {
        /// <summary>
        /// Ottiene la descrizione localizzata del tipo di achievement
        /// </summary>
        public static string GetDescription(this AchievementType type)
        {
            return type switch
            {
                AchievementType.WorkoutsCompleted => "Allenamenti Completati",
                AchievementType.ConsecutiveDays => "Giorni Consecutivi",
                AchievementType.WorkoutStreak => "Streak di Allenamenti",
                AchievementType.TotalWorkoutTime => "Tempo Totale Allenamento",
                AchievementType.WorkoutFrequency => "Frequenza Allenamenti",
                AchievementType.WeightLost => "Peso Perso",
                AchievementType.WeightGained => "Peso Guadagnato",
                AchievementType.BodyFatReduced => "Grasso Corporeo Ridotto",
                AchievementType.MuscleGained => "Massa Muscolare Guadagnata",
                AchievementType.MeasurementReduced => "Circonferenze Ridotte",
                AchievementType.PhotosUploaded => "Foto Caricate",
                AchievementType.PhotoComparison => "Confronti Fotografici",
                AchievementType.TransformationShared => "Trasformazioni Condivise",
                AchievementType.WeeklyGoalMet => "Obiettivi Settimanali",
                AchievementType.MonthlyGoalMet => "Obiettivi Mensili",
                AchievementType.YearlyGoalMet => "Obiettivi Annuali",
                AchievementType.PerfectWeek => "Settimana Perfetta",
                AchievementType.PerfectMonth => "Mese Perfetto",
                AchievementType.PushUpsMaster => "Maestro delle Flessioni",
                AchievementType.SquatChampion => "Campione degli Squat",
                AchievementType.PlankWarrior => "Guerriero del Plank",
                AchievementType.CardioKing => "Re del Cardio",
                AchievementType.StrengthGuru => "Guru della Forza",
                AchievementType.FirstShare => "Prima Condivisione",
                AchievementType.SocialInfluencer => "Influencer Social",
                AchievementType.CommunityMember => "Membro Community",
                AchievementType.Motivator => "Motivatore",
                AchievementType.FirstWorkout => "Primo Allenamento",
                AchievementType.FirstMonth => "Primo Mese",
                AchievementType.FirstYear => "Primo Anno",
                AchievementType.CustomGoalReached => "Obiettivo Personalizzato",
                AchievementType.PersonalRecord => "Record Personale",
                AchievementType.NewYearChallenge => "Sfida di Capodanno",
                AchievementType.SummerBody => "Corpo Estivo",
                AchievementType.HalloweenHorror => "Sfida Halloween",
                AchievementType.ChristmasChallenge => "Sfida Natalizia",
                AchievementType.SpringCleaning => "Pulizie di Primavera",
                AchievementType.ProfileCompleted => "Profilo Completato",
                AchievementType.AllFeatures => "Tutte le Funzionalità",
                AchievementType.DataBackup => "Backup Dati",
                AchievementType.WearableConnected => "Wearable Connesso",
                AchievementType.AppRated => "App Valutata",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Ottiene l'icona associata al tipo di achievement
        /// </summary>
        public static string GetIcon(this AchievementType type)
        {
            return type switch
            {
                AchievementType.WorkoutsCompleted => "💪",
                AchievementType.ConsecutiveDays => "🔥",
                AchievementType.WorkoutStreak => "⚡",
                AchievementType.TotalWorkoutTime => "⏱️",
                AchievementType.WorkoutFrequency => "📊",
                AchievementType.WeightLost => "⚖️",
                AchievementType.WeightGained => "🏋️",
                AchievementType.BodyFatReduced => "📉",
                AchievementType.MuscleGained => "💪",
                AchievementType.MeasurementReduced => "📏",
                AchievementType.PhotosUploaded => "📸",
                AchievementType.PhotoComparison => "🔄",
                AchievementType.TransformationShared => "📤",
                AchievementType.WeeklyGoalMet => "🎯",
                AchievementType.MonthlyGoalMet => "🏆",
                AchievementType.YearlyGoalMet => "👑",
                AchievementType.PerfectWeek => "⭐",
                AchievementType.PerfectMonth => "🌟",
                AchievementType.PushUpsMaster => "🤲",
                AchievementType.SquatChampion => "🦵",
                AchievementType.PlankWarrior => "🪨",
                AchievementType.CardioKing => "❤️",
                AchievementType.StrengthGuru => "🏋️‍♂️",
                AchievementType.FirstShare => "🤝",
                AchievementType.SocialInfluencer => "📱",
                AchievementType.CommunityMember => "👥",
                AchievementType.Motivator => "🎉",
                AchievementType.FirstWorkout => "🎊",
                AchievementType.FirstMonth => "📅",
                AchievementType.FirstYear => "🎂",
                AchievementType.CustomGoalReached => "🎖️",
                AchievementType.PersonalRecord => "🥇",
                AchievementType.NewYearChallenge => "🎆",
                AchievementType.SummerBody => "☀️",
                AchievementType.HalloweenHorror => "🎃",
                AchievementType.ChristmasChallenge => "🎄",
                AchievementType.SpringCleaning => "🌸",
                AchievementType.ProfileCompleted => "✅",
                AchievementType.AllFeatures => "🚀",
                AchievementType.DataBackup => "💾",
                AchievementType.WearableConnected => "⌚",
                AchievementType.AppRated => "⭐",
                _ => "🏅"
            };
        }

        /// <summary>
        /// Ottiene il colore associato al tipo di achievement
        /// </summary>
        public static string GetColor(this AchievementType type)
        {
            return type switch
            {
                // Allenamenti - Blu
                AchievementType.WorkoutsCompleted or
                AchievementType.ConsecutiveDays or
                AchievementType.WorkoutStreak => "#2196F3",

                // Progressi fisici - Verde
                AchievementType.WeightLost or
                AchievementType.BodyFatReduced or
                AchievementType.MeasurementReduced => "#4CAF50",

                // Forza/Muscoli - Rosso
                AchievementType.WeightGained or
                AchievementType.MuscleGained or
                AchievementType.StrengthGuru => "#F44336",

                // Foto/Social - Viola
                AchievementType.PhotosUploaded or
                AchievementType.TransformationShared or
                AchievementType.SocialInfluencer => "#9C27B0",

                // Obiettivi - Arancione
                AchievementType.WeeklyGoalMet or
                AchievementType.MonthlyGoalMet or
                AchievementType.CustomGoalReached => "#FF9800",

                // Speciali/Milestone - Oro
                AchievementType.FirstWorkout or
                AchievementType.FirstYear or
                AchievementType.PersonalRecord => "#FFD700",

                // Stagionali - Variegati
                AchievementType.NewYearChallenge => "#FF5722",
                AchievementType.SummerBody => "#FFC107",
                AchievementType.HalloweenHorror => "#FF5722",
                AchievementType.ChristmasChallenge => "#4CAF50",
                AchievementType.SpringCleaning => "#8BC34A",

                // Default
                _ => "#607D8B"
            };
        }

        /// <summary>
        /// Verifica se l'achievement è stagionale
        /// </summary>
        public static bool IsSeasonal(this AchievementType type)
        {
            return type switch
            {
                AchievementType.NewYearChallenge or
                AchievementType.SummerBody or
                AchievementType.HalloweenHorror or
                AchievementType.ChristmasChallenge or
                AchievementType.SpringCleaning => true,
                _ => false
            };
        }

        /// <summary>
        /// Ottiene il periodo di validità per achievement stagionali
        /// </summary>
        public static (DateTime Start, DateTime End)? GetSeasonalPeriod(this AchievementType type, int year)
        {
            return type switch
            {
                AchievementType.NewYearChallenge => (new DateTime(year, 1, 1), new DateTime(year, 1, 31)),
                AchievementType.SpringCleaning => (new DateTime(year, 3, 20), new DateTime(year, 6, 20)),
                AchievementType.SummerBody => (new DateTime(year, 6, 1), new DateTime(year, 8, 31)),
                AchievementType.HalloweenHorror => (new DateTime(year, 10, 1), new DateTime(year, 10, 31)),
                AchievementType.ChristmasChallenge => (new DateTime(year, 12, 1), new DateTime(year, 12, 31)),
                _ => null
            };
        }
    }
}

// UserGoal.cs - Modello per gli obiettivi dell'utente
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models.Goals
{
    public class UserGoal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Title { get; set; }

        public required string Description { get; set; }

        public GoalType Type { get; set; }
        public GoalPriority Priority { get; set; }
        public GoalStatus Status { get; set; }

        // Valori target
        public float TargetValue { get; set; }
        public float CurrentValue { get; set; }
        public required string Unit { get; set; }

        // Date
        public DateTime CreatedDate { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Relazioni
        public int UserId { get; set; }
        public required User User { get; set; }

        // Proprietà calcolate
        [Ignore]
        public float ProgressPercentage => TargetValue > 0 ? Math.Min((CurrentValue / TargetValue) * 100, 100) : 0;

        [Ignore]
        public bool IsCompleted => Status == GoalStatus.Completed;

        [Ignore]
        public bool IsOverdue => !IsCompleted && DateTime.Now > TargetDate;

        [Ignore]
        public TimeSpan TimeRemaining => TargetDate - DateTime.Now;

        [Ignore]
        public int DaysRemaining => (int)Math.Ceiling(TimeRemaining.TotalDays);
    }

    public enum GoalType
    {
        WeightLoss,         // Perdita di peso
        WeightGain,         // Aumento di peso
        MuscleGain,         // Aumento massa muscolare
        BodyFatReduction,   // Riduzione grasso corporeo
        Workout,            // Obiettivo allenamenti
        Measurement,        // Obiettivo misurazioni
        Strength,           // Obiettivo forza
        Endurance,          // Obiettivo resistenza
        Flexibility,        // Obiettivo flessibilità
        Habit,              // Obiettivo abitudini
        Custom              // Obiettivo personalizzato
    }

    public enum GoalPriority
    {
        Low,        // Bassa priorità
        Medium,     // Media priorità
        High,       // Alta priorità
        Critical    // Priorità critica
    }

    public enum GoalStatus
    {
        NotStarted,     // Non iniziato
        InProgress,     // In corso
        OnHold,         // In pausa
        Completed,      // Completato
        Cancelled,      // Cancellato
        Failed          // Fallito
    }
}

// StatsService.cs - Servizio per calcolare statistiche
using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Goals;

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
            var stats = new UserStats { UserId = userId };

            try
            {
                // Statistiche allenamenti
                var workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(userId);
                stats.TotalWorkoutPlans = workoutPlans.Count;
                stats.ActiveWorkoutPlans = workoutPlans.Count(wp => wp.ModifiedDate >= DateTime.Now.AddDays(-30));

                // Statistiche misurazioni
                var measurements = await _bodyMeasurementRepository.GetByUserIdAsync(userId);
                stats.TotalMeasurements = measurements.Count;

                if(measurements.Any())
                {
                    var latest = measurements.First();
                    var oldest = measurements.Last();
                    stats.CurrentWeight = latest.Weight;
                    stats.WeightChange = latest.Weight - oldest.Weight;
                }

                // Statistiche foto
                var photos = await _progressPhotoRepository.GetByUserIdAsync(userId);
                stats.TotalProgressPhotos = photos.Count;

                // Statistiche achievement
                var achievements = await _achievementRepository.GetByUserIdAsync(userId);
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
            var stats = new WeeklyStats { UserId = userId };

            try
            {
                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);

                // Statistiche della settimana corrente
                var measurements = await _bodyMeasurementRepository.GetByUserIdAndDateRangeAsync(userId, startOfWeek, endOfWeek);
                stats.MeasurementsThisWeek = measurements.Count;

                var photos = await _progressPhotoRepository.GetByUserIdAndDateRangeAsync(userId, startOfWeek, endOfWeek);
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