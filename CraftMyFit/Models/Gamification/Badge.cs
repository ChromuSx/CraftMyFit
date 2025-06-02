// Badge.cs - Modello per i badge di gamification
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models.Gamification
{
    public class Badge
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, System.ComponentModel.DataAnnotations.MaxLength(100)]
        public required string Name { get; set; }

        public required string Description { get; set; }
        public required string IconPath { get; set; }
        public required string Color { get; set; } // Hex color per il badge

        public BadgeCategory Category { get; set; }
        public BadgeRarity Rarity { get; set; }
        public int PointsRequired { get; set; }

        // Condizioni per ottenere il badge
        public BadgeConditionType ConditionType { get; set; }
        public int ConditionValue { get; set; }
        public required string ConditionDescription { get; set; }

        // Relazioni
        public int UserId { get; set; }
        public required User User { get; set; }

        public DateTime? EarnedDate { get; set; }  // null se non ancora ottenuto
        public bool IsVisible { get; set; } = true; // Se il badge deve essere mostrato anche se non ottenuto

        // Proprietà calcolate
        [Ignore]
        public bool IsEarned => EarnedDate.HasValue;

        [Ignore]
        public string DisplayName => IsEarned ? Name : $"??? {Name}";

        [Ignore]
        public string DisplayDescription => IsEarned ? Description : "Badge nascosto - completa le condizioni per sbloccarlo";
    }

    public enum BadgeCategory
    {
        Workout,      // Badge legati agli allenamenti
        Progress,     // Badge legati ai progressi fisici
        Consistency,  // Badge legati alla costanza
        Achievement,  // Badge per achievement specifici
        Social,       // Badge per condivisioni e social
        Special       // Badge speciali per eventi o ricorrenze
    }

    public enum BadgeRarity
    {
        Common,       // Badge comuni, facili da ottenere
        Uncommon,     // Badge non comuni
        Rare,         // Badge rari
        Epic,         // Badge epici
        Legendary     // Badge leggendari, molto difficili
    }

    public enum BadgeConditionType
    {
        WorkoutsCompleted,     // Numero di allenamenti completati
        ConsecutiveDays,       // Giorni consecutivi di allenamento
        WeightLost,           // Peso perso in kg
        PhotosUploaded,       // Foto di progresso caricate
        TimeSpentExercising,  // Tempo totale di esercizio in minuti
        SpecificExercise,     // Esercizio specifico completato N volte
        MilestoneReached,     // Traguardo specifico raggiunto
        SeasonalEvent         // Evento stagionale o speciale
    }
}