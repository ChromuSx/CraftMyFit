using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CraftMyFit.Models.Workout
{
    public class Exercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, System.ComponentModel.DataAnnotations.MaxLength(100)]
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string ImagePath { get; set; }
        public required string VideoPath { get; set; }

        public required string MuscleGroup { get; set; }  // Principale gruppo muscolare coinvolto

        // Memorizza l'equipaggiamento necessario come stringa JSON
        public required string RequiredEquipmentJson { get; set; }

        // Proprietà calcolata (non mappata nel DB)
        [NotMapped]
        public List<string> RequiredEquipment
        {
            get => string.IsNullOrEmpty(RequiredEquipmentJson)
                ? []
                : JsonSerializer.Deserialize<List<string>>(RequiredEquipmentJson);
            set => RequiredEquipmentJson = JsonSerializer.Serialize(value);
        }
    }
}
