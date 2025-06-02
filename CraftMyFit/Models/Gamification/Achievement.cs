using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraftMyFit.Models.Gamification
{
    public class Achievement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, System.ComponentModel.DataAnnotations.MaxLength(100)]
        public required string Title { get; set; }

        public required string Description { get; set; }
        public required string IconPath { get; set; }

        public int PointsAwarded { get; set; }

        public AchievementType Type { get; set; }
        public int TargetValue { get; set; }  // es. numero di allenamenti, giorni consecutivi, ecc.

        // Relazioni
        public int UserId { get; set; }
        public User? User { get; set; }  // Rimosso 'required' - Entity Framework gestirà la relazione

        public DateTime? UnlockedDate { get; set; }  // null se non sbloccato

        [NotMapped]
        public bool IsUnlocked => UnlockedDate.HasValue;
    }
}