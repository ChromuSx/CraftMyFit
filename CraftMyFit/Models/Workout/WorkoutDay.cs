using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models.Workout
{
    public class WorkoutDay
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        [Required, System.ComponentModel.DataAnnotations.MaxLength(100)]
        public required string Title { get; set; }

        public int OrderIndex { get; set; }  // Per mantenere l'ordine dei giorni

        public int WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; } // Nullable per lazy loading

        public ObservableCollection<WorkoutExercise> Exercises { get; set; } = new(); // ObservableCollection per aggiornamento UI
    }
}