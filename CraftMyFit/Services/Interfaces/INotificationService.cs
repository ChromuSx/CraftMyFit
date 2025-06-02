namespace CraftMyFit.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> AreNotificationsEnabledAsync();
        Task<bool> RequestPermissionAsync();
        Task ScheduleWorkoutReminderAsync(int notificationId, string title, string message, DateTime scheduleTime);
        Task ScheduleWeighInReminderAsync(int notificationId, string title, string message, DateTime scheduleTime);
        Task ScheduleCustomReminderAsync(int notificationId, string title, string message, DateTime scheduleTime);
        Task CancelNotificationAsync(int notificationId);
        Task CancelAllNotificationsAsync();
        Task ShowImmediateNotificationAsync(string title, string message);
        Task<List<PendingNotification>> GetPendingNotificationsAsync();

        // Eventi
        event EventHandler<NotificationEventArgs>? NotificationReceived;
        event EventHandler<NotificationEventArgs>? NotificationTapped;
    }

    public class PendingNotification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime ScheduleTime { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class NotificationEventArgs : EventArgs
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string> Data { get; set; } = [];
    }
}