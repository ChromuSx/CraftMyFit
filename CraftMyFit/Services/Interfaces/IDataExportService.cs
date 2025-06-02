namespace CraftMyFit.Services.Interfaces
{
    public interface IDataExportService
    {
        /// <summary>
        /// Esporta tutti i dati dell'utente in formato JSON
        /// </summary>
        Task<string?> ExportAllDataAsJsonAsync(int userId);

        /// <summary>
        /// Esporta tutti i dati dell'utente in formato CSV
        /// </summary>
        Task<string?> ExportAllDataAsCsvAsync(int userId);

        /// <summary>
        /// Esporta le misurazioni corporee in formato CSV
        /// </summary>
        Task<string?> ExportBodyMeasurementsAsync(int userId);

        /// <summary>
        /// Esporta i piani di allenamento in formato JSON
        /// </summary>
        Task<string?> ExportWorkoutPlansAsync(int userId);

        /// <summary>
        /// Esporta le foto di progresso (solo metadati)
        /// </summary>
        Task<string?> ExportProgressPhotosMetadataAsync(int userId);

        /// <summary>
        /// Esporta gli achievement in formato JSON
        /// </summary>
        Task<string?> ExportAchievementsAsync(int userId);

        /// <summary>
        /// Esporta le statistiche in formato leggibile
        /// </summary>
        Task<string?> ExportStatsReportAsync(int userId);

        /// <summary>
        /// Importa dati da un file JSON
        /// </summary>
        Task<bool> ImportDataFromJsonAsync(int userId, string jsonFilePath);

        /// <summary>
        /// Crea un report PDF con tutti i dati (se supportato)
        /// </summary>
        Task<string?> CreatePdfReportAsync(int userId);

        /// <summary>
        /// Verifica l'integrità dei dati esportati
        /// </summary>
        Task<bool> ValidateExportedDataAsync(string filePath);
    }
}