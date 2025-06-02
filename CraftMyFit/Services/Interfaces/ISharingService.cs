using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftMyFit.Services.Interfaces
{
    public interface ISharingService
    {
        /// <summary>
        /// Condivide un testo semplice
        /// </summary>
        Task<bool> ShareTextAsync(string title, string text);

        /// <summary>
        /// Condivide un'immagine con testo
        /// </summary>
        Task<bool> ShareImageAsync(string title, string imagePath, string? text = null);

        /// <summary>
        /// Condivide più immagini
        /// </summary>
        Task<bool> ShareImagesAsync(string title, List<string> imagePaths, string? text = null);

        /// <summary>
        /// Condivide un file
        /// </summary>
        Task<bool> ShareFileAsync(string title, string filePath, string? text = null);

        /// <summary>
        /// Condivide il progresso dell'utente
        /// </summary>
        Task<bool> ShareProgressAsync(string userId, ShareProgressType type);

        /// <summary>
        /// Condivide un piano di allenamento
        /// </summary>
        Task<bool> ShareWorkoutPlanAsync(string workoutPlanId);

        /// <summary>
        /// Condivide un achievement
        /// </summary>
        Task<bool> ShareAchievementAsync(string achievementId);

        /// <summary>
        /// Condivide statistiche personalizzate
        /// </summary>
        Task<bool> ShareCustomStatsAsync(Dictionary<string, object> stats);

        /// <summary>
        /// Crea e condivide un'immagine delle statistiche
        /// </summary>
        Task<bool> ShareStatsImageAsync(string userId);

        /// <summary>
        /// Verifica se la condivisione è disponibile
        /// </summary>
        Task<bool> IsSharingAvailableAsync();
    }

    public enum ShareProgressType
    {
        WeightLoss,
        MuscleGain,
        WorkoutStreak,
        PhotoComparison,
        MonthlyProgress,
        YearlyProgress,
        CustomGoal
    }
}