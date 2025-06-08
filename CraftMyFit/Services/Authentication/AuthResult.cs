// Models/AuthResult.cs
using CraftMyFit.Models;

namespace CraftMyFit.Services.Authentication
{
    /// <summary>
    /// Risultato di un'operazione di autenticazione
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// Indica se l'operazione è riuscita
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Messaggio di errore in caso di fallimento
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Utente autenticato (se l'operazione è riuscita)
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Token di autenticazione
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Token di refresh (opzionale)
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Data di scadenza del token
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Informazioni aggiuntive sull'autenticazione
        /// </summary>
        public Dictionary<string, object>? AdditionalData { get; set; }

        /// <summary>
        /// Crea un risultato di successo
        /// </summary>
        public static AuthResult CreateSuccess(User user, string token, string? refreshToken = null, DateTime? expiresAt = null)
        {
            return new AuthResult
            {
                Success = true,
                User = user,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
        }

        /// <summary>
        /// Crea un risultato di errore
        /// </summary>
        public static AuthResult CreateError(string errorMessage)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Argomenti per l'evento di cambio stato autenticazione
    /// </summary>
    public class AuthStatusChangedEventArgs : EventArgs
    {
        public bool IsAuthenticated { get; set; }
        public User? User { get; set; }
        public string? ErrorMessage { get; set; }
    }
}