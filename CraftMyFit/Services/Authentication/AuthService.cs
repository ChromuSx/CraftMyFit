// AuthService.cs - Servizio di autenticazione
using CraftMyFit.Services.Interfaces;
using System.Text.Json;

namespace CraftMyFit.Services.Authentication
{
    public class AuthService
    {
        private readonly IPreferenceService _preferenceService;
        private readonly IDialogService _dialogService;

        public event EventHandler<AuthStatusChangedEventArgs>? AuthStatusChanged;

        public AuthService(IPreferenceService preferenceService, IDialogService dialogService)
        {
            _preferenceService = preferenceService;
            _dialogService = dialogService;
            LoadStoredAuthData();
        }

        #region Properties

        public bool IsAuthenticated => CurrentUser != null && !string.IsNullOrEmpty(AuthToken);
        public User? CurrentUser { get; private set; }
        public string? AuthToken { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Autentica l'utente con email e password
        /// </summary>
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return new AuthResult { Success = false, ErrorMessage = "Email e password sono obbligatori" };
                }

                // Validazione email
                if(!IsValidEmail(email))
                {
                    return new AuthResult { Success = false, ErrorMessage = "Formato email non valido" };
                }

                // In un'implementazione reale, qui faresti la chiamata API
                // Per ora simuliamo l'autenticazione
                await Task.Delay(1000); // Simula latenza di rete

                // Simulazione autenticazione locale
                if(email == "demo@craftmyfit.com" && password == "demo123")
                {
                    User user = new()
                    {
                        Id = 1,
                        Name = "Demo User",
                        Email = email,
                        RegistrationDate = DateTime.Now.AddDays(-30),
                        IsEmailVerified = true
                    };

                    string token = GenerateMockJwtToken(user);

                    await SetAuthenticatedUserAsync(user, token);

                    return new AuthResult { Success = true, User = user, Token = token };
                }

                return new AuthResult { Success = false, ErrorMessage = "Credenziali non valide" };
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel login: {ex.Message}");
                return new AuthResult { Success = false, ErrorMessage = "Errore durante l'autenticazione" };
            }
        }

        /// <summary>
        /// Registra un nuovo utente
        /// </summary>
        public async Task<AuthResult> RegisterAsync(string name, string email, string password)
        {
            try
            {
                // Validazioni
                if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return new AuthResult { Success = false, ErrorMessage = "Tutti i campi sono obbligatori" };
                }

                if(!IsValidEmail(email))
                {
                    return new AuthResult { Success = false, ErrorMessage = "Formato email non valido" };
                }

                if(password.Length < 6)
                {
                    return new AuthResult { Success = false, ErrorMessage = "La password deve essere di almeno 6 caratteri" };
                }

                // Simulazione registrazione
                await Task.Delay(1000);

                User user = new()
                {
                    Id = new Random().Next(1000, 9999),
                    Name = name,
                    Email = email,
                    RegistrationDate = DateTime.Now,
                    IsEmailVerified = false
                };

                string token = GenerateMockJwtToken(user);

                await SetAuthenticatedUserAsync(user, token);

                return new AuthResult { Success = true, User = user, Token = token };
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella registrazione: {ex.Message}");
                return new AuthResult { Success = false, ErrorMessage = "Errore durante la registrazione" };
            }
        }

        /// <summary>
        /// Logout dell'utente corrente
        /// </summary>
        public async Task<bool> LogoutAsync()
        {
            try
            {
                // Pulisce i dati locali
                CurrentUser = null;
                AuthToken = null;

                // Rimuove i dati salvati
                _preferenceService.Remove("auth_user");
                _preferenceService.Remove("auth_token");
                _preferenceService.Remove("auth_expires");

                // Notifica il cambio di stato
                AuthStatusChanged?.Invoke(this, new AuthStatusChangedEventArgs { IsAuthenticated = false });

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel logout: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Rinnova il token di autenticazione
        /// </summary>
        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                if(string.IsNullOrEmpty(AuthToken))
                {
                    return false;
                }

                // In implementazione reale, qui chiameresti l'endpoint di refresh
                await Task.Delay(500);

                // Simula rinnovo token
                if(CurrentUser != null)
                {
                    string newToken = GenerateMockJwtToken(CurrentUser);
                    AuthToken = newToken;

                    _preferenceService.SetString("auth_token", AuthToken);
                    _preferenceService.SetDateTime("auth_expires", DateTime.Now.AddDays(7));

                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel rinnovo token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica se il token è ancora valido
        /// </summary>
        public async Task<bool> IsTokenValidAsync()
        {
            try
            {
                if(string.IsNullOrEmpty(AuthToken))
                {
                    return false;
                }

                DateTime expiryDate = _preferenceService.GetDateTime("auth_expires", DateTime.MinValue);
                if(expiryDate <= DateTime.Now)
                {
                    // Token scaduto, prova a rinnovarlo
                    return await RefreshTokenAsync();
                }

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella validazione token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Invia email di reset password
        /// </summary>
        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                if(!IsValidEmail(email))
                {
                    return false;
                }

                // Simulazione invio email
                await Task.Delay(1000);

                await _dialogService.ShowAlertAsync("Email Inviata",
                    "Ti abbiamo inviato un'email con le istruzioni per reimpostare la password.");

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'invio email reset: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cambia la password dell'utente corrente
        /// </summary>
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                if(!IsAuthenticated)
                {
                    return false;
                }

                if(string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return false;
                }

                if(newPassword.Length < 6)
                {
                    await _dialogService.ShowAlertAsync("Errore", "La nuova password deve essere di almeno 6 caratteri");
                    return false;
                }

                // Simulazione cambio password
                await Task.Delay(1000);

                await _dialogService.ShowAlertAsync("Successo", "Password cambiata con successo");
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel cambio password: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Private Methods

        private void LoadStoredAuthData()
        {
            try
            {
                string userJson = _preferenceService.GetString("auth_user");
                string token = _preferenceService.GetString("auth_token");
                DateTime expiryDate = _preferenceService.GetDateTime("auth_expires", DateTime.MinValue);

                if(!string.IsNullOrEmpty(userJson) && !string.IsNullOrEmpty(token) && expiryDate > DateTime.Now)
                {
                    CurrentUser = JsonSerializer.Deserialize<User>(userJson);
                    AuthToken = token;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento dati auth: {ex.Message}");
            }
        }

        private async Task SetAuthenticatedUserAsync(User user, string token)
        {
            CurrentUser = user;
            AuthToken = token;

            // Salva i dati
            string userJson = JsonSerializer.Serialize(user);
            _preferenceService.SetString("auth_user", userJson);
            _preferenceService.SetString("auth_token", token);
            _preferenceService.SetDateTime("auth_expires", DateTime.Now.AddDays(7));

            // Notifica il cambio di stato
            AuthStatusChanged?.Invoke(this, new AuthStatusChangedEventArgs
            {
                IsAuthenticated = true,
                User = user
            });

            await Task.CompletedTask;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static string GenerateMockJwtToken(User user) =>
            // In un'implementazione reale, il token JWT sarebbe generato dal server
            // Qui creiamo un token fittizio per scopi dimostrativi
            $"mock_jwt_token_{user.Id}_{DateTime.Now.Ticks}";

        #endregion
    }

    // Modelli per l'autenticazione
    public class AuthResult
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class AuthStatusChangedEventArgs : EventArgs
    {
        public bool IsAuthenticated { get; set; }
        public User? User { get; set; }
    }

    // Estensione del modello User per l'autenticazione
    public partial class User
    {
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
    }
}

// ApiEndpoints.cs - Definizione degli endpoint API
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