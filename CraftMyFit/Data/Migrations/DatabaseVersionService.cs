using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftMyFit.Data.Migrations
{
    public class DatabaseVersionService
    {
        private readonly CraftMyFitDbContext _context;
        private readonly ILogger<DatabaseVersionService> _logger;

        public DatabaseVersionService(CraftMyFitDbContext context, ILogger<DatabaseVersionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task MigrateAsync()
        {
            // Ottieni la versione attuale del database
            int currentVersion = await GetCurrentVersionAsync();

            // Esegui le migrazioni necessarie
            if(currentVersion < 1)
            {
                await MigrateToVersion1Async();
                await SetVersionAsync(1);
            }

            if(currentVersion < 2)
            {
                await MigrateToVersion2Async();
                await SetVersionAsync(2);
            }

            // Aggiungi ulteriori migrazioni quando necessario
        }

        private async Task<int> GetCurrentVersionAsync()
        {
            // Verifica se esiste la tabella delle versioni
            bool tableExists = await _context.Database.ExecuteSqlRawAsync("SELECT name FROM sqlite_master WHERE type='table' AND name='DatabaseVersion'") > 0;

            if(!tableExists)
            {
                await _context.Database.ExecuteSqlRawAsync("CREATE TABLE DatabaseVersion (Version INT NOT NULL)");
                await _context.Database.ExecuteSqlRawAsync("INSERT INTO DatabaseVersion (Version) VALUES (0)");
                return 0;
            }

            // Ottieni la versione attuale
            var version = await _context.Database.SqlQuery<int>("SELECT Version FROM DatabaseVersion LIMIT 1").FirstOrDefaultAsync();
            return version;
        }

        private async Task SetVersionAsync(int version)
        {
            await _context.Database.ExecuteSqlRawAsync($"UPDATE DatabaseVersion SET Version = {version}");
        }

        private async Task MigrateToVersion1Async()
        {
            // Implementa le migrazioni necessarie per la versione 1
            _logger.LogInformation("Migrazione alla versione 1 del database");
        }

        private async Task MigrateToVersion2Async()
        {
            // Implementa le migrazioni necessarie per la versione 2
            _logger.LogInformation("Migrazione alla versione 2 del database");
        }
    }
}
