using CraftMyFit.Data;
using CraftMyFit.Data.Initialization;
using CraftMyFit.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Helpers.Utils
{
    public static class DatabaseSetup
    {
        /// <summary>
        /// Inizializza il database dell'applicazione
        /// </summary>
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();
                var preferenceService = scope.ServiceProvider.GetRequiredService<IPreferenceService>();

                var initializer = new DatabaseInitializer(context, preferenceService);
                await initializer.InitializeAsync();

                System.Diagnostics.Debug.WriteLine("Database inizializzato con successo");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione del database: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Controlla se è necessario aggiornare il database
        /// </summary>
        public static async Task<bool> NeedsDatabaseUpdateAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                return pendingMigrations.Any();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo aggiornamenti database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Esegue il backup del database
        /// </summary>
        public static async Task<bool> BackupDatabaseAsync(IServiceProvider serviceProvider, string backupPath)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();
                var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

                // Ottieni il path del database corrente
                var connectionString = context.Database.GetConnectionString();
                if(string.IsNullOrEmpty(connectionString))
                    return false;

                // Estrai il path del file database dalla connection string
                var dbPath = ExtractDatabasePath(connectionString);
                if(string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
                    return false;

                // Crea la directory di backup se non esiste
                var backupDirectory = Path.GetDirectoryName(backupPath);
                if(!string.IsNullOrEmpty(backupDirectory))
                {
                    await fileService.CreateDirectoryAsync(backupDirectory);
                }

                // Copia il file database
                return await fileService.CopyAsync(dbPath, backupPath);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel backup del database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ripristina il database da un backup
        /// </summary>
        public static async Task<bool> RestoreDatabaseAsync(IServiceProvider serviceProvider, string backupPath)
        {
            try
            {
                if(!File.Exists(backupPath))
                    return false;

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();
                var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

                // Ottieni il path del database corrente
                var connectionString = context.Database.GetConnectionString();
                if(string.IsNullOrEmpty(connectionString))
                    return false;

                var dbPath = ExtractDatabasePath(connectionString);
                if(string.IsNullOrEmpty(dbPath))
                    return false;

                // Chiudi la connessione al database
                await context.Database.CloseConnectionAsync();

                // Sostituisci il database corrente con il backup
                return await fileService.CopyAsync(backupPath, dbPath);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel ripristino del database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Pulisce i dati obsoleti dal database
        /// </summary>
        public static async Task CleanupOldDataAsync(IServiceProvider serviceProvider, int daysToKeep = 365)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();

                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                // Rimuovi le sessioni di allenamento molto vecchie (mantieni quelle più recenti)
                var oldSessions = await context.WorkoutSessions
                    .Where(ws => ws.StartTime < cutoffDate)
                    .ToListAsync();

                if(oldSessions.Any())
                {
                    context.WorkoutSessions.RemoveRange(oldSessions);
                }

                // Mantieni solo le misurazioni più significative per date molto vecchie
                // (Ad esempio, una per mese per date oltre 1 anno fa)
                var veryOldDate = DateTime.Now.AddYears(-1);
                var oldMeasurements = await context.BodyMeasurements
                    .Where(bm => bm.Date < veryOldDate)
                    .GroupBy(bm => new { bm.UserId, Year = bm.Date.Year, Month = bm.Date.Month })
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g.OrderBy(bm => bm.Date).Skip(1))
                    .ToListAsync();

                if(oldMeasurements.Any())
                {
                    context.BodyMeasurements.RemoveRange(oldMeasurements);
                }

                await context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"Cleanup completato: rimossi {oldSessions.Count} sessioni e {oldMeasurements.Count} misurazioni obsolete");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel cleanup del database: {ex.Message}");
            }
        }

        /// <summary>
        /// Ottiene le statistiche del database
        /// </summary>
        public static async Task<DatabaseStats> GetDatabaseStatsAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();

                var stats = new DatabaseStats
                {
                    UsersCount = await context.Users.CountAsync(),
                    WorkoutPlansCount = await context.WorkoutPlans.CountAsync(),
                    ExercisesCount = await context.Exercises.CountAsync(),
                    WorkoutSessionsCount = await context.WorkoutSessions.CountAsync(),
                    BodyMeasurementsCount = await context.BodyMeasurements.CountAsync(),
                    ProgressPhotosCount = await context.ProgressPhotos.CountAsync(),
                    AchievementsCount = await context.Achievements.CountAsync(),
                    DatabaseSizeBytes = await GetDatabaseSizeAsync(context)
                };

                return stats;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero statistiche database: {ex.Message}");
                return new DatabaseStats();
            }
        }

        /// <summary>
        /// Verifica l'integrità del database
        /// </summary>
        public static async Task<bool> VerifyDatabaseIntegrityAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();

                // Esegui alcune query di base per verificare l'integrità
                var usersCount = await context.Users.CountAsync();
                var exercisesCount = await context.Exercises.CountAsync();
                var workoutPlansCount = await context.WorkoutPlans.CountAsync();

                // Verifica che le relazioni fondamentali siano intatte
                var orphanedWorkoutPlans = await context.WorkoutPlans
                    .Where(wp => wp.User == null)
                    .CountAsync();

                if(orphanedWorkoutPlans > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Trovati {orphanedWorkoutPlans} piani di allenamento orfani");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine("Verifica integrità database completata con successo");
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella verifica integrità database: {ex.Message}");
                return false;
            }
        }

        private static string? ExtractDatabasePath(string connectionString)
        {
            // Estrae il path del database dalla connection string SQLite
            var dataSourcePrefix = "Data Source=";
            var startIndex = connectionString.IndexOf(dataSourcePrefix, StringComparison.OrdinalIgnoreCase);

            if(startIndex == -1)
                return null;

            startIndex += dataSourcePrefix.Length;
            var endIndex = connectionString.IndexOf(';', startIndex);

            return endIndex == -1
                ? connectionString[startIndex..]
                : connectionString[startIndex..endIndex];
        }

        private static async Task<long> GetDatabaseSizeAsync(CraftMyFitDbContext context)
        {
            try
            {
                var connectionString = context.Database.GetConnectionString();
                if(string.IsNullOrEmpty(connectionString))
                    return 0;

                var dbPath = ExtractDatabasePath(connectionString);
                if(string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
                    return 0;

                var fileInfo = new FileInfo(dbPath);
                return fileInfo.Length;
            }
            catch
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Statistiche del database
    /// </summary>
    public class DatabaseStats
    {
        public int UsersCount { get; set; }
        public int WorkoutPlansCount { get; set; }
        public int ExercisesCount { get; set; }
        public int WorkoutSessionsCount { get; set; }
        public int BodyMeasurementsCount { get; set; }
        public int ProgressPhotosCount { get; set; }
        public int AchievementsCount { get; set; }
        public long DatabaseSizeBytes { get; set; }

        public string DatabaseSizeFormatted => FormatBytes(DatabaseSizeBytes);

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while(len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}