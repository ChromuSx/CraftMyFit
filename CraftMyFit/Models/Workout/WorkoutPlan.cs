using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CraftMyFit.Models.Workout
{
    public class WorkoutPlan
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, System.ComponentModel.DataAnnotations.MaxLength(100)]
        public required string Title { get; set; }

        public required string Description { get; set; }

        // Memorizza i giorni della settimana come stringa JSON
        public required string WorkoutDaysJson { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; } // Nullable perché può essere caricata con lazy loading

        public List<WorkoutDay> WorkoutDays { get; set; } = []; // Inizializzata ma non required

        // Proprietà calcolata (non mappata nel DB)
        [NotMapped]
        public List<DayOfWeek> WorkoutDaysEnum
        {
            get => string.IsNullOrEmpty(WorkoutDaysJson)
                ? []
                : JsonSerializer.Deserialize<List<DayOfWeek>>(WorkoutDaysJson);
            set => WorkoutDaysJson = JsonSerializer.Serialize(value);
        }
    }
}