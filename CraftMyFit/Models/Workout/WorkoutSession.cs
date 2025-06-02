using CraftMyFit.Models.Progress;
using SQLite;

namespace CraftMyFit.Models.Workout
{
    public class WorkoutSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public float? CaloriesBurned { get; set; }
        public float? AverageHeartRate { get; set; }

        // Relazioni
        public int WorkoutPlanId { get; set; }
        public required WorkoutPlan WorkoutPlan { get; set; }

        public int WorkoutDayId { get; set; }
        public required WorkoutDay WorkoutDay { get; set; }

        public int UserId { get; set; }
        public required User User { get; set; }

        public required List<ExerciseLog> ExerciseLogs { get; set; }

        public required string Notes { get; set; }
    }
}
