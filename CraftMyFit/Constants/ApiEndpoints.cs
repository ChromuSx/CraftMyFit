namespace CraftMyFit.Constants
{
    public static class ApiEndpoints
    {
        // Base URL dell'API
        public const string BaseUrl = "https://api.craftmyfit.com/v1";

        // Endpoint di autenticazione
        public static class Auth
        {
            public const string Login = BaseUrl + "/auth/login";
            public const string Register = BaseUrl + "/auth/register";
            public const string Logout = BaseUrl + "/auth/logout";
            public const string RefreshToken = BaseUrl + "/auth/refresh";
            public const string ForgotPassword = BaseUrl + "/auth/forgot-password";
            public const string ResetPassword = BaseUrl + "/auth/reset-password";
            public const string ChangePassword = BaseUrl + "/auth/change-password";
            public const string VerifyEmail = BaseUrl + "/auth/verify-email";
        }

        // Endpoint utente
        public static class User
        {
            public const string Profile = BaseUrl + "/user/profile";
            public const string UpdateProfile = BaseUrl + "/user/profile";
            public const string DeleteAccount = BaseUrl + "/user/delete";
            public const string UploadAvatar = BaseUrl + "/user/avatar";
            public const string GetStats = BaseUrl + "/user/stats";
        }

        // Endpoint esercizi
        public static class Exercises
        {
            public const string GetAll = BaseUrl + "/exercises";
            public const string GetById = BaseUrl + "/exercises/{id}";
            public const string Search = BaseUrl + "/exercises/search";
            public const string GetByMuscleGroup = BaseUrl + "/exercises/muscle-group/{muscleGroup}";
            public const string Create = BaseUrl + "/exercises";
            public const string Update = BaseUrl + "/exercises/{id}";
            public const string Delete = BaseUrl + "/exercises/{id}";
            public const string UploadImage = BaseUrl + "/exercises/{id}/image";
            public const string UploadVideo = BaseUrl + "/exercises/{id}/video";
        }

        // Endpoint piani di allenamento
        public static class WorkoutPlans
        {
            public const string GetAll = BaseUrl + "/workout-plans";
            public const string GetById = BaseUrl + "/workout-plans/{id}";
            public const string Create = BaseUrl + "/workout-plans";
            public const string Update = BaseUrl + "/workout-plans/{id}";
            public const string Delete = BaseUrl + "/workout-plans/{id}";
            public const string Duplicate = BaseUrl + "/workout-plans/{id}/duplicate";
            public const string Share = BaseUrl + "/workout-plans/{id}/share";
            public const string GetPublic = BaseUrl + "/workout-plans/public";
        }

        // Endpoint sessioni di allenamento
        public static class WorkoutSessions
        {
            public const string Start = BaseUrl + "/workout-sessions";
            public const string Update = BaseUrl + "/workout-sessions/{id}";
            public const string Complete = BaseUrl + "/workout-sessions/{id}/complete";
            public const string Cancel = BaseUrl + "/workout-sessions/{id}/cancel";
            public const string GetHistory = BaseUrl + "/workout-sessions/history";
            public const string GetStats = BaseUrl + "/workout-sessions/stats";
        }

        // Endpoint progressi
        public static class Progress
        {
            public const string BodyMeasurements = BaseUrl + "/progress/measurements";
            public const string ProgressPhotos = BaseUrl + "/progress/photos";
            public const string UploadPhoto = BaseUrl + "/progress/photos/upload";
            public const string DeletePhoto = BaseUrl + "/progress/photos/{id}";
            public const string GetStats = BaseUrl + "/progress/stats";
            public const string GetCharts = BaseUrl + "/progress/charts";
        }

        // Endpoint achievement/gamification
        public static class Achievements
        {
            public const string GetAll = BaseUrl + "/achievements";
            public const string GetUnlocked = BaseUrl + "/achievements/unlocked";
            public const string GetProgress = BaseUrl + "/achievements/progress";
            public const string Unlock = BaseUrl + "/achievements/{id}/unlock";
        }

        // Endpoint dispositivi wearable
        public static class Wearables
        {
            public const string Connect = BaseUrl + "/wearables/connect";
            public const string Disconnect = BaseUrl + "/wearables/disconnect";
            public const string Sync = BaseUrl + "/wearables/sync";
            public const string GetData = BaseUrl + "/wearables/data";
        }

        // Endpoint sincronizzazione
        public static class Sync
        {
            public const string Upload = BaseUrl + "/sync/upload";
            public const string Download = BaseUrl + "/sync/download";
            public const string GetLastSync = BaseUrl + "/sync/last";
            public const string Backup = BaseUrl + "/sync/backup";
            public const string Restore = BaseUrl + "/sync/restore";
        }

        // Endpoint notifiche
        public static class Notifications
        {
            public const string Register = BaseUrl + "/notifications/register";
            public const string Unregister = BaseUrl + "/notifications/unregister";
            public const string UpdatePreferences = BaseUrl + "/notifications/preferences";
            public const string GetHistory = BaseUrl + "/notifications/history";
        }

        // Helper methods per costruire URL dinamici
        public static string GetExerciseById(int id) => Exercises.GetById.Replace("{id}", id.ToString());
        public static string GetWorkoutPlanById(int id) => WorkoutPlans.GetById.Replace("{id}", id.ToString());
        public static string UpdateWorkoutPlan(int id) => WorkoutPlans.Update.Replace("{id}", id.ToString());
        public static string DeleteWorkoutPlan(int id) => WorkoutPlans.Delete.Replace("{id}", id.ToString());
        public static string DuplicateWorkoutPlan(int id) => WorkoutPlans.Duplicate.Replace("{id}", id.ToString());
        public static string ShareWorkoutPlan(int id) => WorkoutPlans.Share.Replace("{id}", id.ToString());
        public static string GetExercisesByMuscleGroup(string muscleGroup) =>
            Exercises.GetByMuscleGroup.Replace("{muscleGroup}", Uri.EscapeDataString(muscleGroup));
        public static string UpdateWorkoutSession(int id) => WorkoutSessions.Update.Replace("{id}", id.ToString());
        public static string CompleteWorkoutSession(int id) => WorkoutSessions.Complete.Replace("{id}", id.ToString());
        public static string CancelWorkoutSession(int id) => WorkoutSessions.Cancel.Replace("{id}", id.ToString());
        public static string DeleteProgressPhoto(int id) => Progress.DeletePhoto.Replace("{id}", id.ToString());
        public static string UnlockAchievement(int id) => Achievements.Unlock.Replace("{id}", id.ToString());
        public static string UploadExerciseImage(int id) => Exercises.UploadImage.Replace("{id}", id.ToString());
        public static string UploadExerciseVideo(int id) => Exercises.UploadVideo.Replace("{id}", id.ToString());
        public static string UpdateExercise(int id) => Exercises.Update.Replace("{id}", id.ToString());
        public static string DeleteExercise(int id) => Exercises.Delete.Replace("{id}", id.ToString());

        // Parametri query comuni
        public static class QueryParams
        {
            public const string Search = "search";
            public const string Page = "page";
            public const string PageSize = "pageSize";
            public const string SortBy = "sortBy";
            public const string SortOrder = "sortOrder";
            public const string StartDate = "startDate";
            public const string EndDate = "endDate";
            public const string MuscleGroup = "muscleGroup";
            public const string Equipment = "equipment";
            public const string Difficulty = "difficulty";
            public const string IncludeDeleted = "includeDeleted";
        }

        // Headers comuni
        public static class Headers
        {
            public const string Authorization = "Authorization";
            public const string ContentType = "Content-Type";
            public const string Accept = "Accept";
            public const string UserAgent = "User-Agent";
            public const string ApiVersion = "X-API-Version";
            public const string DeviceId = "X-Device-Id";
            public const string AppVersion = "X-App-Version";
            public const string Platform = "X-Platform";
        }
    }
}