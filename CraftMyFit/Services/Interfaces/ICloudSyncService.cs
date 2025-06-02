// ICloudSyncService.cs - Interfaccia per la sincronizzazione cloud
namespace CraftMyFit.Services.Interfaces
{
    public interface ICloudSyncService
    {
        /// <summary>
        /// Verifica se la sincronizzazione cloud è abilitata
        /// </summary>
        Task<bool> IsCloudSyncEnabledAsync();

        /// <summary>
        /// Abilita la sincronizzazione cloud
        /// </summary>
        Task<bool> EnableCloudSyncAsync();

        /// <summary>
        /// Disabilita la sincronizzazione cloud
        /// </summary>
        Task<bool> DisableCloudSyncAsync();

        /// <summary>
        /// Sincronizza tutti i dati con il cloud
        /// </summary>
        Task<SyncResult> SyncAllDataAsync();

        /// <summary>
        /// Sincronizza solo i dati modificati
        /// </summary>
        Task<SyncResult> SyncChangesAsync();

        /// <summary>
        /// Carica i dati dal cloud
        /// </summary>
        Task<SyncResult> DownloadFromCloudAsync();

        /// <summary>
        /// Carica i dati sul cloud
        /// </summary>
        Task<SyncResult> UploadToCloudAsync();

        /// <summary>
        /// Ottiene l'ultima data di sincronizzazione
        /// </summary>
        Task<DateTime?> GetLastSyncDateAsync();

        /// <summary>
        /// Verifica se ci sono conflitti di sincronizzazione
        /// </summary>
        Task<List<SyncConflict>> CheckForConflictsAsync();

        /// <summary>
        /// Risolve i conflitti di sincronizzazione
        /// </summary>
        Task<bool> ResolveConflictsAsync(List<SyncConflictResolution> resolutions);

        /// <summary>
        /// Crea un backup completo nel cloud
        /// </summary>
        Task<bool> CreateBackupAsync(string backupName);

        /// <summary>
        /// Ripristina da un backup nel cloud
        /// </summary>
        Task<bool> RestoreFromBackupAsync(string backupId);

        /// <summary>
        /// Ottiene la lista dei backup disponibili
        /// </summary>
        Task<List<CloudBackup>> GetAvailableBackupsAsync();

        /// <summary>
        /// Eventi per notificare lo stato della sincronizzazione
        /// </summary>
        event EventHandler<SyncStatusChangedEventArgs>? SyncStatusChanged;
        event EventHandler<SyncProgressEventArgs>? SyncProgressChanged;
    }

    #region Modelli per la sincronizzazione

    public class SyncResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int ItemsSynced { get; set; }
        public int ItemsSkipped { get; set; }
        public int ConflictsFound { get; set; }
        public DateTime SyncCompletedAt { get; set; }
        public TimeSpan SyncDuration { get; set; }
        public List<string> SyncedTables { get; set; } = [];
    }

    public class SyncConflict
    {
        public string Id { get; set; } = string.Empty;
        public SyncConflictType Type { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public DateTime LocalModifiedDate { get; set; }
        public DateTime CloudModifiedDate { get; set; }
        public string LocalData { get; set; } = string.Empty;
        public string CloudData { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public enum SyncConflictType
    {
        DataMismatch,       // Dati diversi per lo stesso elemento
        DeletedLocally,     // Elemento eliminato localmente ma modificato nel cloud
        DeletedInCloud,     // Elemento eliminato nel cloud ma modificato localmente
        NewInBoth,          // Nuovo elemento creato sia localmente che nel cloud
        TimestampMismatch   // Conflitto di timestamp
    }

    public class SyncConflictResolution
    {
        public string ConflictId { get; set; } = string.Empty;
        public SyncResolutionAction Action { get; set; }
        public string? CustomData { get; set; }
    }

    public enum SyncResolutionAction
    {
        KeepLocal,      // Mantieni la versione locale
        KeepCloud,      // Mantieni la versione cloud
        Merge,          // Unisci le versioni
        KeepBoth,       // Mantieni entrambe le versioni
        SkipItem        // Ignora questo elemento
    }

    public class CloudBackup
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public long SizeBytes { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAutomatic { get; set; }
        public int DataVersion { get; set; }
    }

    public class SyncStatusChangedEventArgs : EventArgs
    {
        public SyncStatus Status { get; set; }
        public string? Message { get; set; }
    }

    public class SyncProgressEventArgs : EventArgs
    {
        public int TotalItems { get; set; }
        public int ProcessedItems { get; set; }
        public string CurrentOperation { get; set; } = string.Empty;
        public float ProgressPercentage => TotalItems > 0 ? (float)ProcessedItems / TotalItems * 100 : 0;
    }

    public enum SyncStatus
    {
        Idle,           // Inattivo
        Connecting,     // Connessione in corso
        Uploading,      // Caricamento in corso
        Downloading,    // Scaricamento in corso
        Processing,     // Elaborazione in corso
        Completed,      // Completato
        Failed,         // Fallito
        Cancelled       // Cancellato
    }

    #endregion
}