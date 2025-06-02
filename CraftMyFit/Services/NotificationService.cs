using CraftMyFit.Services.Interfaces;

namespace CraftMyFit.Services
{
    public class NotificationService : INotificationService
    {
        private readonly List<PendingNotification> _pendingNotifications = [];

        public event EventHandler<NotificationEventArgs>? NotificationReceived;
        public event EventHandler<NotificationEventArgs>? NotificationTapped;

        public async Task<bool> AreNotificationsEnabledAsync()
        {
            try
            {
                // In un'implementazione reale, questo controllerebbe le impostazioni del sistema
                // Per ora restituiamo sempre true
                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo delle notifiche: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RequestPermissionAsync()
        {
            try
            {
                // In un'implementazione reale, questo richiederebbe i permessi al sistema
                // Per ora restituiamo sempre true
                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella richiesta permessi: {ex.Message}");
                return false;
            }
        }

        public async Task ScheduleWorkoutReminderAsync(int notificationId, string title, string message, DateTime scheduleTime)
        {
            try
            {
                var notification = new PendingNotification
                {
                    Id = notificationId,
                    Title = title,
                    Message = message,
                    ScheduleTime = scheduleTime,
                    Type = "workout_reminder"
                };

                await ScheduleNotificationInternal(notification);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella programmazione promemoria allenamento: {ex.Message}");
            }
        }

        public async Task ScheduleWeighInReminderAsync(int notificationId, string title, string message, DateTime scheduleTime)
        {
            try
            {
                var notification = new PendingNotification
                {
                    Id = notificationId,
                    Title = title,
                    Message = message,
                    ScheduleTime = scheduleTime,
                    Type = "weigh_in_reminder"
                };

                await ScheduleNotificationInternal(notification);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella programmazione promemoria pesata: {ex.Message}");
            }
        }

        public async Task ScheduleCustomReminderAsync(int notificationId, string title, string message, DateTime scheduleTime)
        {
            try
            {
                var notification = new PendingNotification
                {
                    Id = notificationId,
                    Title = title,
                    Message = message,
                    ScheduleTime = scheduleTime,
                    Type = "custom_reminder"
                };

                await ScheduleNotificationInternal(notification);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella programmazione promemoria personalizzato: {ex.Message}");
            }
        }

        public async Task CancelNotificationAsync(int notificationId)
        {
            try
            {
                // Rimuovi dalla lista delle notifiche pending
                var notification = _pendingNotifications.FirstOrDefault(n => n.Id == notificationId);
                if(notification != null)
                {
                    _ = _pendingNotifications.Remove(notification);
                }

                // In un'implementazione reale, qui cancelleresti la notifica dal sistema
                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'annullamento notifica: {ex.Message}");
            }
        }

        public async Task CancelAllNotificationsAsync()
        {
            try
            {
                _pendingNotifications.Clear();

                // In un'implementazione reale, qui cancelleresti tutte le notifiche dal sistema
                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'annullamento di tutte le notifiche: {ex.Message}");
            }
        }

        public async Task ShowImmediateNotificationAsync(string title, string message)
        {
            try
            {
                // In un'implementazione reale, questo mostrerebbe una notifica immediata
                // Per ora simuliamo l'evento
                var eventArgs = new NotificationEventArgs
                {
                    NotificationId = 0,
                    Title = title,
                    Message = message,
                    Data = new Dictionary<string, string> { { "immediate", "true" } }
                };

                NotificationReceived?.Invoke(this, eventArgs);
                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella notifica immediata: {ex.Message}");
            }
        }

        public async Task<List<PendingNotification>> GetPendingNotificationsAsync()
        {
            try
            {
                // Rimuovi le notifiche scadute
                DateTime now = DateTime.Now;
                _ = _pendingNotifications.RemoveAll(n => n.ScheduleTime <= now);

                return await Task.FromResult(_pendingNotifications.ToList());
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero notifiche pending: {ex.Message}");
                return [];
            }
        }

        private async Task ScheduleNotificationInternal(PendingNotification notification)
        {
            // Rimuovi eventuali notifiche esistenti con lo stesso ID
            await CancelNotificationAsync(notification.Id);

            // Aggiungi la nuova notifica se è futura
            if(notification.ScheduleTime > DateTime.Now)
            {
                _pendingNotifications.Add(notification);

                // In un'implementazione reale, qui programmeresti la notifica nel sistema
                System.Diagnostics.Debug.WriteLine($"Notifica programmata: {notification.Title} per {notification.ScheduleTime}");
            }
        }

        #region Helper Methods per Testing

        /// <summary>
        /// Simula la ricezione di una notifica (per testing)
        /// </summary>
        public void SimulateNotificationReceived(int notificationId, string title, string message)
        {
            var eventArgs = new NotificationEventArgs
            {
                NotificationId = notificationId,
                Title = title,
                Message = message
            };

            NotificationReceived?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Simula il tap su una notifica (per testing)
        /// </summary>
        public void SimulateNotificationTapped(int notificationId, string title, string message, Dictionary<string, string>? data = null)
        {
            var eventArgs = new NotificationEventArgs
            {
                NotificationId = notificationId,
                Title = title,
                Message = message,
                Data = data ?? []
            };

            NotificationTapped?.Invoke(this, eventArgs);
        }

        #endregion

        #region Metodi di Utilità

        public static string GenerateWorkoutReminderMessage(string workoutName)
        {
            string[] messages = new[]
            {
                $"È ora del tuo allenamento: {workoutName}! 💪",
                $"Il tuo allenamento {workoutName} ti sta aspettando! 🏋️",
                $"Tempo di allenarsi con {workoutName}! 🔥",
                $"Non dimenticare il tuo allenamento: {workoutName}! ⏰"
            };

            Random random = new();
            return messages[random.Next(messages.Length)];
        }

        public static string GenerateWeighInReminderMessage()
        {
            string[] messages = new[]
            {
                "È ora di pesarsi! Controlla i tuoi progressi! ⚖️",
                "Promemoria: registra il tuo peso oggi! 📊",
                "Non dimenticare di pesarti e registrare i progressi! 📈",
                "Tempo di monitorare i tuoi progressi! ⚖️"
            };

            Random random = new();
            return messages[random.Next(messages.Length)];
        }

        public static string GenerateMotivationalTitle()
        {
            string[] titles = new[]
            {
                "Resta Motivato! 💪",
                "CraftMyFit ti ricorda",
                "È il momento! 🔥",
                "Il tuo fitness conta! ⭐"
            };

            Random random = new();
            return titles[random.Next(titles.Length)];
        }

        #endregion
    }
}