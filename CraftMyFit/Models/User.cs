using CraftMyFit.Models.Gamification;
using CraftMyFit.Models.Progress;
using CraftMyFit.Models.Workout;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public required string Name { get; set; }

        public string? ProfileImagePath { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Informazioni aggiuntive del profilo
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } // "M", "F", "Other"
        public float? Height { get; set; } // in cm
        public float? StartingWeight { get; set; } // in kg
        public string? FitnessGoal { get; set; } // "lose_weight", "gain_muscle", "maintain", "improve_fitness"
        public string? ActivityLevel { get; set; } // "sedentary", "light", "moderate", "active", "very_active"

        // Impostazioni preferenze
        public string PreferredWeightUnit { get; set; } = "kg";
        public string PreferredLengthUnit { get; set; } = "cm";

        // Relazioni (inizializzate come liste vuote per evitare null reference)
        public List<WorkoutPlan> WorkoutPlans { get; set; } = [];
        public List<ProgressPhoto> ProgressPhotos { get; set; } = [];
        public List<BodyMeasurement> BodyMeasurements { get; set; } = [];
        public List<Achievement> Achievements { get; set; } = [];

        // Proprietà calcolate
        [Ignore]
        public int Age => DateOfBirth?.CalculateAge() ?? 0;

        [Ignore]
        public float? CurrentWeight => BodyMeasurements?
            .OrderByDescending(bm => bm.Date)
            .FirstOrDefault()?.Weight;

        [Ignore]
        public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : "Utente";
    }
}

// Estensione per calcolare l'età
public static class DateTimeExtensionsForAge
{
    public static int CalculateAge(this DateTime birthDate)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        if(birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}