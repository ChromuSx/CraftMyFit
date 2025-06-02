using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftMyFit.Models.Workout
{
    public class WorkoutExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int OrderIndex { get; set; }  // Per mantenere l'ordine degli esercizi

        public int Sets { get; set; }
        public int Reps { get; set; }
        public float Weight { get; set; }

        public TimeSpan Duration { get; set; }
        public TimeSpan RestTime { get; set; }

        // Relazioni
        public int WorkoutDayId { get; set; }
        public WorkoutDay WorkoutDay { get; set; }

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
    }
}
