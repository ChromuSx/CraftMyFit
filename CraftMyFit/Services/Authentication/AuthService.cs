// AuthService.cs - Servizio di autenticazione
using CraftMyFit.Models;
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
}