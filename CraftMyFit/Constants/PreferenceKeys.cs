namespace CraftMyFit.Constants
{
    public static class PreferenceKeys
    {
        // User preferences
        public const string CurrentUserId = "current_user_id";
        public const string UserName = "user_name";
        public const string UserProfileImage = "user_profile_image";
        public const string FirstTimeUser = "first_time_user";

        // App settings
        public const string PreferredWeightUnit = "preferred_weight_unit"; // kg, lbs
        public const string PreferredLengthUnit = "preferred_length_unit"; // cm, inches
        public const string Theme = "app_theme"; // light, dark, system
        public const string Language = "app_language";

        // Workout preferences
        public const string DefaultRestTime = "default_rest_time";
        public const string PlaySounds = "play_sounds";
        public const string AutoStartTimer = "auto_start_timer";
        public const string ShowWorkoutReminders = "show_workout_reminders";

        // Notification preferences
        public const string EnableNotifications = "enable_notifications";
        public const string WorkoutReminderTime = "workout_reminder_time";
        public const string WeighInReminderEnabled = "weigh_in_reminder_enabled";
        public const string WeighInReminderTime = "weigh_in_reminder_time";

        // Privacy settings
        public const string AllowDataCollection = "allow_data_collection";
        public const string ShareProgressPhotos = "share_progress_photos";

        // Backup settings
        public const string LastBackupDate = "last_backup_date";
        public const string AutoBackupEnabled = "auto_backup_enabled";
    }
}