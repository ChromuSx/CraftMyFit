using CraftMyFit.Models;
using CraftMyFit.Services.Interfaces;
using System.Text.Json;

namespace CraftMyFit.Services.Authentication
{
    public class AuthService
    {
        private readonly INavigationService _navigationService;
        private readonly IPreferenceService _preferenceService;
        private readonly IDialogService _dialogService;

        public event EventHandler<AuthStatusChangedEventArgs>? AuthStatusChanged;

        public AuthService(INavigationService navigationService, IPreferenceService preferenceService, IDialogService dialogService)
        {
            _navigationService = navigationService;
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

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                // Logica di login esistente
                if (email == "demo@craftmyfit.com" && password == "demo123")
                {
                    // Salva stato di autenticazione
                    _preferenceService.SetBool("IsLoggedIn", true);
                    _preferenceService.SetString("UserEmail", email);

                    // Naviga alla dashboard
                    await _navigationService.NavigateToAsync("//dashboard");

                    return new AuthResult { Success = true };
                }

                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Credenziali non valide"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = $"Errore durante il login: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> LogoutAsync()
        {
            try
            {
                // Rimuovi dati di autenticazione
                _preferenceService.Remove("IsLoggedIn");
                _preferenceService.Remove("UserEmail");

                // Pulisci altri dati se necessario
                // _preferenceService.Remove("AuthToken");

                // Naviga alla pagina di login
                await _navigationService.NavigateToAsync("//login");

                return new AuthResult { Success = true };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = $"Errore durante il logout: {ex.Message}"
                };
            }
        }

        public bool IsLoggedIn()
        {
            return _preferenceService.GetBool("IsLoggedIn", false);
        }

        /// <summary>
        /// Verifica se il token è ancora valido
        /// </summary>
        public async Task<bool> IsTokenValidAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(AuthToken))
                {
                    return false;
                }

                // Controlla se il token è scaduto localmente
                DateTime expirationDate = _preferenceService.GetDateTime("auth_expires", DateTime.MinValue);
                if (expirationDate != DateTime.MinValue && DateTime.Now >= expirationDate)
                {
                    System.Diagnostics.Debug.WriteLine("Token scaduto localmente");
                    return false;
                }

                // In implementazione reale, qui verificheresti il token con il server
                await Task.Delay(500);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella validazione token: {ex.Message}");
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
                if (string.IsNullOrEmpty(AuthToken))
                {
                    return false;
                }

                await Task.Delay(500);

                if (CurrentUser != null)
                {
                    string newToken = GenerateMockJwtToken(CurrentUser);
                    AuthToken = newToken;

                    _preferenceService.SetString("auth_token", AuthToken);
                    _preferenceService.SetDateTime("auth_expires", DateTime.Now.AddHours(24));

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel refresh token: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Private Methods

        private void LoadStoredAuthData()
        {
            try
            {
                string userJson = _preferenceService.GetString("auth_user", string.Empty);
                string token = _preferenceService.GetString("auth_token", string.Empty);

                if (!string.IsNullOrEmpty(userJson) && !string.IsNullOrEmpty(token))
                {
                    CurrentUser = JsonSerializer.Deserialize<User>(userJson);
                    AuthToken = token;

                    System.Diagnostics.Debug.WriteLine($"Dati di autenticazione caricati per utente: {CurrentUser?.Email}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento dati autenticazione: {ex.Message}");

                // Pulisci i dati corrotti
                _preferenceService.Remove("auth_user");
                _preferenceService.Remove("auth_token");
                _preferenceService.Remove("auth_expires");
            }
        }

        private string GenerateMockJwtToken(User user)
        {
            // Token JWT fittizio per simulazione
            return $"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{{\"sub\":\"{user.Id}\",\"email\":\"{user.Email}\",\"exp\":{DateTimeOffset.Now.AddHours(24).ToUnixTimeSeconds()}}}"))}";
        }

        #endregion
    }
}