namespace CraftMyFit.Services.Interfaces
{
    public interface IWearableDeviceService
    {
        /// <summary>
        /// Verifica se ci sono dispositivi wearable connessi
        /// </summary>
        Task<bool> IsConnectedAsync();

        /// <summary>
        /// Ottiene la lista dei dispositivi wearable disponibili
        /// </summary>
        Task<List<WearableDevice>> GetAvailableDevicesAsync();

        /// <summary>
        /// Connette a un dispositivo wearable specifico
        /// </summary>
        Task<bool> ConnectToDeviceAsync(string deviceId);

        /// <summary>
        /// Disconnette dal dispositivo wearable corrente
        /// </summary>
        Task<bool> DisconnectAsync();

        /// <summary>
        /// Sincronizza i dati dal dispositivo wearable
        /// </summary>
        Task<WearableData?> SyncDataAsync();

        /// <summary>
        /// Ottiene i dati della frequenza cardiaca in tempo reale
        /// </summary>
        Task<int?> GetCurrentHeartRateAsync();

        /// <summary>
        /// Ottiene il numero di passi della giornata
        /// </summary>
        Task<int?> GetTodayStepsAsync();

        /// <summary>
        /// Ottiene le calorie bruciate della giornata
        /// </summary>
        Task<float?> GetTodayCaloriesAsync();

        /// <summary>
        /// Invia una notifica al dispositivo wearable
        /// </summary>
        Task<bool> SendNotificationAsync(string title, string message);

        /// <summary>
        /// Avvia il monitoraggio di un allenamento sul dispositivo
        /// </summary>
        Task<bool> StartWorkoutTrackingAsync(string workoutType);

        /// <summary>
        /// Ferma il monitoraggio dell'allenamento
        /// </summary>
        Task<bool> StopWorkoutTrackingAsync();

        /// <summary>
        /// Ottiene i dati dell'allenamento corrente
        /// </summary>
        Task<WorkoutTrackingData?> GetCurrentWorkoutDataAsync();

        /// <summary>
        /// Eventi per notificare cambiamenti di stato
        /// </summary>
        event EventHandler<WearableDeviceStatusChangedEventArgs>? DeviceStatusChanged;
        event EventHandler<WearableDataReceivedEventArgs>? DataReceived;
    }

    #region Modelli dati

    public class WearableDevice
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public WearableDeviceType Type { get; set; }
        public bool IsConnected { get; set; }
        public int BatteryLevel { get; set; }
        public DateTime LastSync { get; set; }
    }

    public enum WearableDeviceType
    {
        Unknown,
        AppleWatch,
        Fitbit,
        GarminWatch,
        SamsungWatch,
        WearOS
    }

    public class WearableData
    {
        public int? HeartRate { get; set; }
        public int Steps { get; set; }
        public float CaloriesBurned { get; set; }
        public float DistanceKm { get; set; }
        public int ActiveMinutes { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = [];
    }

    public class WorkoutTrackingData
    {
        public string WorkoutType { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int? AverageHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
        public float CaloriesBurned { get; set; }
        public float DistanceKm { get; set; }
        public List<HeartRateDataPoint> HeartRateHistory { get; set; } = [];
    }

    public class HeartRateDataPoint
    {
        public DateTime Timestamp { get; set; }
        public int HeartRate { get; set; }
    }

    public class WearableDeviceStatusChangedEventArgs : EventArgs
    {
        public WearableDevice Device { get; set; } = new();
        public bool IsConnected { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }

    public class WearableDataReceivedEventArgs : EventArgs
    {
        public WearableData Data { get; set; } = new();
        public string DeviceId { get; set; } = string.Empty;
    }

    #endregion
}