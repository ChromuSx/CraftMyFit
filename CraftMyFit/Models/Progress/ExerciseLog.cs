using CraftMyFit.Models.Workout;
using SQLite;

namespace CraftMyFit.Models.Progress
{
    public class ExerciseLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int SetsCompleted { get; set; }
        public int RepsCompleted { get; set; }
        public float WeightUsed { get; set; }

        // Relazioni
        public int WorkoutSessionId { get; set; }
        public required WorkoutSession WorkoutSession { get; set; }

        public int ExerciseId { get; set; }
        public required Exercise Exercise { get; set; }
    }
}
