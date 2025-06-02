using CraftMyFit.Models.Gamification;
using CraftMyFit.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Initialization
{
    public class DatabaseInitializer
    {
        private readonly CraftMyFitDbContext _context;
        public readonly IPreferenceService _preferenceService;

        public DatabaseInitializer(CraftMyFitDbContext context, IPreferenceService preferenceService)
        {
            _context = context;
            _preferenceService = preferenceService;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Crea il database se non exists
                _ = await _context.Database.EnsureCreatedAsync();

                // Applica eventuali migrazioni pending
                if((await _context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _context.Database.MigrateAsync();
                }

                // Inizializza dati di base se è la prima volta
                await SeedInitialDataAsync();
            }
            catch(Exception ex)
            {
                // Log dell'errore (in futuro con un logger appropriato)
                System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione del database: {ex.Message}");
                throw;
            }
        }

        private async Task SeedInitialDataAsync()
        {
            // Verifica se è la prima volta che l'app viene avviata
            bool isFirstTimeUser = _preferenceService.GetBool("first_time_user", true);

            if(!isFirstTimeUser)
            {
                return;
            }

            try
            {
                // Crea achievement predefiniti se non esistono
                await CreateDefaultAchievementsAsync();

                // Marca che non è più la prima volta
                _preferenceService.SetBool("first_time_user", false);

                _ = await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel seeding dei dati iniziali: {ex.Message}");
            }
        }

        private async Task CreateDefaultAchievementsAsync()
        {
            // Verifica se esistono già achievement
            if(await _context.Achievements.AnyAsync())
            {
                return;
            }

            List<Achievement> defaultAchievements =
            [
                new()
                {
                    Title = "Primo Allenamento",
                    Description = "Completa il tuo primo allenamento",
                    IconPath = "achievement_first_workout.png",
                    PointsAwarded = 10,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 1,
                    UserId = 0 // Sarà assegnato quando l'utente viene creato
                },
                new()
                {
                    Title = "Costanza Principiante",
                    Description = "Completa 5 allenamenti",
                    IconPath = "achievement_5_workouts.png",
                    PointsAwarded = 25,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 5,
                    UserId = 0
                },
                new()
                {
                    Title = "Guerriero del Fitness",
                    Description = "Completa 10 allenamenti",
                    IconPath = "achievement_10_workouts.png",
                    PointsAwarded = 50,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 10,
                    UserId = 0
                },
                new()
                {
                    Title = "Prima Foto",
                    Description = "Carica la tua prima foto di progresso",
                    IconPath = "achievement_first_photo.png",
                    PointsAwarded = 15,
                    Type = AchievementType.PhotosUploaded,
                    TargetValue = 1,
                    UserId = 0
                },
                new()
                {
                    Title = "Una Settimana",
                    Description = "Allenati per 7 giorni consecutivi",
                    IconPath = "achievement_7_days.png",
                    PointsAwarded = 100,
                    Type = AchievementType.ConsecutiveDays,
                    TargetValue = 7,
                    UserId = 0
                }
            ];

            // Nota: Questi achievement template verranno copiati per ogni nuovo utente
            // Per ora li salviamo come template con UserId = 0
            // _context.Achievements.AddRange(defaultAchievements);
        }

        public async Task CreateUserAchievementsAsync(int userId)
        {
            // Crea achievement personalizzati per un nuovo utente
            List<Achievement> userAchievements =
            [
                new()
                {
                    Title = "Primo Allenamento",
                    Description = "Completa il tuo primo allenamento",
                    IconPath = "achievement_first_workout.png",
                    PointsAwarded = 10,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 1,
                    UserId = userId
                },
                new()
                {
                    Title = "Costanza Principiante",
                    Description = "Completa 5 allenamenti",
                    IconPath = "achievement_5_workouts.png",
                    PointsAwarded = 25,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 5,
                    UserId = userId
                },
                new()
                {
                    Title = "Guerriero del Fitness",
                    Description = "Completa 10 allenamenti",
                    IconPath = "achievement_10_workouts.png",
                    PointsAwarded = 50,
                    Type = AchievementType.WorkoutsCompleted,
                    TargetValue = 10,
                    UserId = userId
                },
                new()
                {
                    Title = "Prima Foto",
                    Description = "Carica la tua prima foto di progresso",
                    IconPath = "achievement_first_photo.png",
                    PointsAwarded = 15,
                    Type = AchievementType.PhotosUploaded,
                    TargetValue = 1,
                    UserId = userId
                },
                new()
                {
                    Title = "Una Settimana",
                    Description = "Allenati per 7 giorni consecutivi",
                    IconPath = "achievement_7_days.png",
                    PointsAwarded = 100,
                    Type = AchievementType.ConsecutiveDays,
                    TargetValue = 7,
                    UserId = userId
                }
            ];

            _context.Achievements.AddRange(userAchievements);
            _ = await _context.SaveChangesAsync();
        }

        public async Task<bool> DatabaseExistsAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task ResetDatabaseAsync()
        {
            try
            {
                _ = await _context.Database.EnsureDeletedAsync();
                await InitializeAsync();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel reset del database: {ex.Message}");
                throw;
            }
        }
    }
}