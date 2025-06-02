using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models.Gamification
{
    namespace CraftMyFit.Models.Goals
    {
        public class UserGoal
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            [Required, System.ComponentModel.DataAnnotations.MaxLength(100)]
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
            public float ProgressPercentage => TargetValue > 0 ? Math.Min(CurrentValue / TargetValue * 100, 100) : 0;

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
}
