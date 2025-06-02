using CraftMyFit.Models;
using CraftMyFit.Models.Gamification;
using CraftMyFit.Models.Progress;
using CraftMyFit.Models.Workout;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CraftMyFit.Data
{
    public class CraftMyFitDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutDay> WorkoutDays { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<ProgressPhoto> ProgressPhotos { get; set; }
        public DbSet<BodyMeasurement> BodyMeasurements { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<ExerciseLog> ExerciseLogs { get; set; }

        public CraftMyFitDbContext(DbContextOptions<CraftMyFitDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazione delle relazioni

            // User -> WorkoutPlan (uno a molti)
            _ = modelBuilder.Entity<WorkoutPlan>()
                .HasOne(wp => wp.User)
                .WithMany(u => u.WorkoutPlans)
                .HasForeignKey(wp => wp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutPlan -> WorkoutDay (uno a molti)
            _ = modelBuilder.Entity<WorkoutDay>()
                .HasOne(wd => wd.WorkoutPlan)
                .WithMany(wp => wp.WorkoutDays)
                .HasForeignKey(wd => wd.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutDay -> WorkoutExercise (uno a molti)
            _ = modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.WorkoutDay)
                .WithMany(wd => wd.Exercises)
                .HasForeignKey(we => we.WorkoutDayId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exercise -> WorkoutExercise (uno a molti)
            _ = modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany()
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> ProgressPhoto (uno a molti)
            _ = modelBuilder.Entity<ProgressPhoto>()
                .HasOne(pp => pp.User)
                .WithMany(u => u.ProgressPhotos)
                .HasForeignKey(pp => pp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> BodyMeasurement (uno a molti)
            _ = modelBuilder.Entity<BodyMeasurement>()
                .HasOne(bm => bm.User)
                .WithMany(u => u.BodyMeasurements)
                .HasForeignKey(bm => bm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Achievement (uno a molti)
            _ = modelBuilder.Entity<Achievement>()
                .HasOne(a => a.User)
                .WithMany(u => u.Achievements)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutSession relazioni
            _ = modelBuilder.Entity<WorkoutSession>()
                .HasOne(ws => ws.User)
                .WithMany()
                .HasForeignKey(ws => ws.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<WorkoutSession>()
                .HasOne(ws => ws.WorkoutPlan)
                .WithMany()
                .HasForeignKey(ws => ws.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = modelBuilder.Entity<WorkoutSession>()
                .HasOne(ws => ws.WorkoutDay)
                .WithMany()
                .HasForeignKey(ws => ws.WorkoutDayId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExerciseLog relazioni
            _ = modelBuilder.Entity<ExerciseLog>()
                .HasOne(el => el.WorkoutSession)
                .WithMany(ws => ws.ExerciseLogs)
                .HasForeignKey(el => el.WorkoutSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<ExerciseLog>()
                .HasOne(el => el.Exercise)
                .WithMany()
                .HasForeignKey(el => el.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indici per migliorare le performance
            _ = modelBuilder.Entity<WorkoutPlan>()
                .HasIndex(wp => wp.UserId);

            _ = modelBuilder.Entity<ProgressPhoto>()
                .HasIndex(pp => pp.UserId);

            _ = modelBuilder.Entity<BodyMeasurement>()
                .HasIndex(bm => bm.UserId);

            _ = modelBuilder.Entity<BodyMeasurement>()
                .HasIndex(bm => bm.Date);

            _ = modelBuilder.Entity<WorkoutSession>()
                .HasIndex(ws => ws.UserId);

            _ = modelBuilder.Entity<WorkoutSession>()
                .HasIndex(ws => ws.StartTime);

            // Esempi di dati per il database
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder) =>
            // Aggiunge alcuni esercizi predefiniti
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    Name = "Push-up",
                    Description = "Esercizio per pettorali, tricipiti e spalle. Posizionati a terra con le mani alla larghezza delle spalle.",
                    MuscleGroup = "Pettorali",
                    ImagePath = "",
                    VideoPath = "",
                    RequiredEquipmentJson = JsonSerializer.Serialize(new List<string>())
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Squat",
                    Description = "Esercizio per quadricipiti, glutei e core. Piedi alla larghezza delle spalle, scendi come se ti sedessi.",
                    MuscleGroup = "Gambe",
                    ImagePath = "",
                    VideoPath = "",
                    RequiredEquipmentJson = JsonSerializer.Serialize(new List<string>())
                },
                new Exercise
                {
                    Id = 3,
                    Name = "Plank",
                    Description = "Esercizio isometrico per il core. Mantieni la posizione per il tempo prestabilito.",
                    MuscleGroup = "Addominali",
                    ImagePath = "",
                    VideoPath = "",
                    RequiredEquipmentJson = JsonSerializer.Serialize(new List<string>())
                },
                new Exercise
                {
                    Id = 4,
                    Name = "Burpee",
                    Description = "Esercizio completo per tutto il corpo. Combina squat, plank e salto.",
                    MuscleGroup = "Full Body",
                    ImagePath = "",
                    VideoPath = "",
                    RequiredEquipmentJson = JsonSerializer.Serialize(new List<string>())
                },
                new Exercise
                {
                    Id = 5,
                    Name = "Bicep Curl",
                    Description = "Esercizio per i bicipiti. Solleva i pesi piegando i gomiti.",
                    MuscleGroup = "Bicipiti",
                    ImagePath = "",
                    VideoPath = "",
                    RequiredEquipmentJson = JsonSerializer.Serialize(new List<string> { "Manubri", "Bilanciere" })
                }
            );
    }
}