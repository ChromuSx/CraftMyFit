using SQLite;

namespace CraftMyFit.Models.Progress
{
    public class BodyMeasurement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        // Misurazioni principali
        public float Weight { get; set; }  // in kg
        public float? BodyFatPercentage { get; set; }  // opzionale
        public float? MuscleMass { get; set; }  // in kg, opzionale

        // Misurazioni specifiche (tutte in cm)
        public float? Chest { get; set; }
        public float? Waist { get; set; }
        public float? Hips { get; set; }
        public float? LeftArm { get; set; }
        public float? RightArm { get; set; }
        public float? LeftThigh { get; set; }
        public float? RightThigh { get; set; }
        public float? LeftCalf { get; set; }
        public float? RightCalf { get; set; }

        // Note aggiuntive
        public required string Notes { get; set; }

        // Relazioni
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}
