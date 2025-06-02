using CraftMyFit.Data;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Migrations
{
    public class DatabaseVersionService
    {
        private readonly CraftMyFitDbContext _context;
        private const int CurrentDatabaseVersion = 1;

        public DatabaseVersionService(CraftMyFitDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica se è necessario aggiornare il database
        /// </summary>
        public async Task<bool> NeedsUpdateAsync()
        {
            try
            {
                var currentVersion = await GetCurrentVersionAsync();
                return currentVersion < CurrentDatabaseVersion;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo versione database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ottiene la versione corrente del database
        /// </summary>
        public async Task<int> GetCurrentVersionAsync()
        {
            try
            {
                // Verifica se la tabella delle versioni esiste
                var tableExists = await _context.Database.SqlQueryRaw<int>(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='DatabaseVersion'")
                    .FirstOrDefaultAsync();

                if(tableExists == 0)
                {
                    // Prima installazione
                    return 0;
                }

                // Ottieni l'ultima versione
                var version = await _context.Database.SqlQueryRaw<int>(
                    "SELECT Version FROM DatabaseVersion ORDER BY Id DESC LIMIT 1")
                    .FirstOrDefaultAsync();

                return version;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero versione: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Esegue le migrazioni necessarie
        /// </summary>
        public async Task<bool> MigrateAsync()
        {
            try
            {
                var currentVersion = await GetCurrentVersionAsync();

                // Esegui le migrazioni in sequenza
                for(int version = currentVersion + 1; version <= CurrentDatabaseVersion; version++)
                {
                    bool success = await ExecuteMigrationAsync(version);
                    if(!success)
                    {
                        System.Diagnostics.Debug.WriteLine($"Migrazione fallita alla versione {version}");
                        return false;
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nelle migrazioni: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Crea la struttura iniziale del database
        /// </summary>
        public async Task<bool> CreateInitialStructureAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();

                // Crea la tabella delle versioni se non esiste
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS DatabaseVersion (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Version INTEGER NOT NULL,
                        AppliedDate DATETIME NOT NULL,
                        Description TEXT
                    )");

                // Inserisci la versione iniziale
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO DatabaseVersion (Version, AppliedDate, Description) 
                    VALUES (1, datetime('now'), 'Initial database structure')");

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione struttura iniziale: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ExecuteMigrationAsync(int version)
        {
            try
            {
                switch(version)
                {
                    case 1:
                        // Migrazione alla versione 1 - struttura iniziale
                        await CreateInitialStructureAsync();
                        break;

                    // Aggiungi qui le future migrazioni
                    // case 2:
                    //     await MigrateToVersion2Async();
                    //     break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"Migrazione non definita per la versione {version}");
                        return false;
                }

                // Registra la migrazione completata
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO DatabaseVersion (Version, AppliedDate, Description) 
                    VALUES ({0}, datetime('now'), {1})",
                    version, $"Migration to version {version}");

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella migrazione versione {version}: {ex.Message}");
                return false;
            }
        }
    }
}