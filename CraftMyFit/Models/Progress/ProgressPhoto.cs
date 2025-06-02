using SQLite;

namespace CraftMyFit.Models.Progress
{
    public class ProgressPhoto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public required string PhotoPath { get; set; }
        public DateTime Date { get; set; }
        public required string Description { get; set; }

        // Relazioni
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}
