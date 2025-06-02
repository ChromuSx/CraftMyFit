using System.Globalization;
using System.Text.RegularExpressions;

namespace CraftMyFit.Helpers.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converte la prima lettera in maiuscolo
        /// </summary>
        public static string Capitalize(this string input) => string.IsNullOrEmpty(input) ? input : char.ToUpper(input[0]) + input[1..].ToLower();

        /// <summary>
        /// Converte in Title Case
        /// </summary>
        public static string ToTitleCase(this string input) => string.IsNullOrEmpty(input) ? input : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());

        /// <summary>
        /// Verifica se la stringa è un email valida
        /// </summary>
        public static bool IsValidEmail(this string email)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tronca la stringa alla lunghezza specificata aggiungendo "..."
        /// </summary>
        public static string Truncate(this string input, int maxLength) => string.IsNullOrEmpty(input) || input.Length <= maxLength ? input : input[..(maxLength - 3)] + "...";

        /// <summary>
        /// Rimuove caratteri speciali mantenendo solo lettere, numeri e spazi
        /// </summary>
        public static string RemoveSpecialCharacters(this string input) => string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"[^a-zA-Z0-9\s]", "");

        /// <summary>
        /// Converte in formato slug (per URL)
        /// </summary>
        public static string ToSlug(this string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Converti in minuscolo
            input = input.ToLower();

            // Sostituisci spazi con trattini
            input = Regex.Replace(input, @"\s+", "-");

            // Rimuovi caratteri speciali
            input = Regex.Replace(input, @"[^a-z0-9\-]", "");

            // Rimuovi trattini multipli
            input = Regex.Replace(input, @"-+", "-");

            // Rimuovi trattini all'inizio e alla fine
            input = input.Trim('-');

            return input;
        }

        /// <summary>
        /// Conta il numero di parole
        /// </summary>
        public static int WordCount(this string input) => string.IsNullOrWhiteSpace(input)
                ? 0
                : input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

        /// <summary>
        /// Verifica se la stringa contiene solo numeri
        /// </summary>
        public static bool IsNumeric(this string input) => !string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit);

        /// <summary>
        /// Formatta una durata in secondi in formato leggibile
        /// </summary>
        public static string FormatDuration(this int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            if(timeSpan.TotalHours >= 1)
            {
                return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            else
            {
                return timeSpan.TotalMinutes >= 1 ? $"{timeSpan.Minutes}m {timeSpan.Seconds}s" : $"{timeSpan.Seconds}s";
            }
        }

        /// <summary>
        /// Maschera parte di una stringa (utile per email, telefoni, etc.)
        /// </summary>
        public static string Mask(this string input, int visibleStart = 2, int visibleEnd = 2, char maskChar = '*')
        {
            if(string.IsNullOrEmpty(input) || input.Length <= visibleStart + visibleEnd)
            {
                return input;
            }

            string start = input[..visibleStart];
            string end = input[^visibleEnd..];
            int maskLength = input.Length - visibleStart - visibleEnd;
            string mask = new(maskChar, maskLength);

            return start + mask + end;
        }
    }
}