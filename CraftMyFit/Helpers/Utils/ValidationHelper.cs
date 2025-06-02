using System.Text.RegularExpressions;

namespace CraftMyFit.Helpers.Utils
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida un indirizzo email
        /// </summary>
        public static bool IsValidEmail(string email)
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
        /// Valida una password
        /// </summary>
        public static ValidationResult ValidatePassword(string password)
        {
            ValidationResult result = new();

            if(string.IsNullOrWhiteSpace(password))
            {
                result.AddError("La password è obbligatoria");
                return result;
            }

            if(password.Length < 6)
            {
                result.AddError("La password deve essere di almeno 6 caratteri");
            }

            if(password.Length > 100)
            {
                result.AddError("La password non può superare i 100 caratteri");
            }

            // Verifica che contenga almeno una lettera e un numero
            if(!password.Any(char.IsLetter))
            {
                result.AddError("La password deve contenere almeno una lettera");
            }

            if(!password.Any(char.IsDigit))
            {
                result.AddError("La password deve contenere almeno un numero");
            }

            return result;
        }

        /// <summary>
        /// Valida un nome utente
        /// </summary>
        public static ValidationResult ValidateUsername(string username)
        {
            ValidationResult result = new();

            if(string.IsNullOrWhiteSpace(username))
            {
                result.AddError("Il nome utente è obbligatorio");
                return result;
            }

            if(username.Length < 2)
            {
                result.AddError("Il nome utente deve essere di almeno 2 caratteri");
            }

            if(username.Length > 50)
            {
                result.AddError("Il nome utente non può superare i 50 caratteri");
            }

            // Verifica che contenga solo caratteri validi
            if(!Regex.IsMatch(username, @"^[a-zA-Z0-9_\s]+$"))
            {
                result.AddError("Il nome utente può contenere solo lettere, numeri, underscore e spazi");
            }

            return result;
        }

        /// <summary>
        /// Valida il peso corporeo
        /// </summary>
        public static ValidationResult ValidateWeight(float weight, string unit = "kg")
        {
            ValidationResult result = new();

            if(weight <= 0)
            {
                result.AddError("Il peso deve essere maggiore di zero");
                return result;
            }

            switch(unit.ToLower())
            {
                case "kg":
                    if(weight is < 20 or > 300)
                    {
                        result.AddError("Il peso deve essere compreso tra 20 e 300 kg");
                    }

                    break;

                case "lbs":
                    if(weight is < 44 or > 660)
                    {
                        result.AddError("Il peso deve essere compreso tra 44 e 660 lbs");
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Valida l'altezza
        /// </summary>
        public static ValidationResult ValidateHeight(float height, string unit = "cm")
        {
            ValidationResult result = new();

            if(height <= 0)
            {
                result.AddError("L'altezza deve essere maggiore di zero");
                return result;
            }

            switch(unit.ToLower())
            {
                case "cm":
                    if(height is < 100 or > 250)
                    {
                        result.AddError("L'altezza deve essere compresa tra 100 e 250 cm");
                    }

                    break;

                case "in":
                    if(height is < 39 or > 98)
                    {
                        result.AddError("L'altezza deve essere compresa tra 39 e 98 pollici");
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Valida una misurazione corporea
        /// </summary>
        public static ValidationResult ValidateBodyMeasurement(float measurement, string measurementType, string unit = "cm")
        {
            ValidationResult result = new();

            if(measurement <= 0)
            {
                result.AddError($"La misurazione {measurementType} deve essere maggiore di zero");
                return result;
            }

            // Limiti ragionevoli per le misurazioni corporee
            switch(unit.ToLower())
            {
                case "cm":
                    if(measurement is < 10 or > 200)
                    {
                        result.AddError($"La misurazione {measurementType} deve essere compresa tra 10 e 200 cm");
                    }

                    break;

                case "in":
                    if(measurement is < 4 or > 78)
                    {
                        result.AddError($"La misurazione {measurementType} deve essere compresa tra 4 e 78 pollici");
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Valida parametri di allenamento (serie, ripetizioni, peso)
        /// </summary>
        public static ValidationResult ValidateWorkoutParameters(int sets, int reps, float weight)
        {
            ValidationResult result = new();

            // Valida le serie
            if(sets is < 1 or > 20)
            {
                result.AddError("Le serie devono essere comprese tra 1 e 20");
            }

            // Valida le ripetizioni
            if(reps is < 1 or > 100)
            {
                result.AddError("Le ripetizioni devono essere comprese tra 1 e 100");
            }

            // Valida il peso (può essere 0 per esercizi a corpo libero)
            if(weight is < 0 or > 500)
            {
                result.AddError("Il peso deve essere compreso tra 0 e 500 kg");
            }

            return result;
        }

        /// <summary>
        /// Valida la durata di un esercizio
        /// </summary>
        public static ValidationResult ValidateExerciseDuration(TimeSpan duration)
        {
            ValidationResult result = new();

            if(duration.TotalSeconds < 1)
            {
                result.AddError("La durata deve essere di almeno 1 secondo");
            }

            if(duration.TotalHours > 2)
            {
                result.AddError("La durata non può superare le 2 ore");
            }

            return result;
        }

        /// <summary>
        /// Valida un titolo di piano di allenamento
        /// </summary>
        public static ValidationResult ValidateWorkoutPlanTitle(string title)
        {
            ValidationResult result = new();

            if(string.IsNullOrWhiteSpace(title))
            {
                result.AddError("Il titolo del piano è obbligatorio");
                return result;
            }

            if(title.Length < 3)
            {
                result.AddError("Il titolo deve essere di almeno 3 caratteri");
            }

            if(title.Length > 100)
            {
                result.AddError("Il titolo non può superare i 100 caratteri");
            }

            return result;
        }

        /// <summary>
        /// Valida una descrizione
        /// </summary>
        public static ValidationResult ValidateDescription(string description, int maxLength = 500)
        {
            ValidationResult result = new();

            if(!string.IsNullOrEmpty(description) && description.Length > maxLength)
            {
                result.AddError($"La descrizione non può superare i {maxLength} caratteri");
            }

            return result;
        }

        /// <summary>
        /// Valida una data
        /// </summary>
        public static ValidationResult ValidateDate(DateTime date, bool allowFuture = true, bool allowPast = true)
        {
            ValidationResult result = new();
            DateTime today = DateTime.Today;

            if(!allowPast && date.Date < today)
            {
                result.AddError("La data non può essere nel passato");
            }

            if(!allowFuture && date.Date > today)
            {
                result.AddError("La data non può essere nel futuro");
            }

            // Verifica che la data non sia troppo lontana nel passato o futuro
            DateTime hundredYearsAgo = today.AddYears(-100);
            DateTime hundredYearsFromNow = today.AddYears(100);

            if(date < hundredYearsAgo || date > hundredYearsFromNow)
            {
                result.AddError("La data non è valida");
            }

            return result;
        }

        /// <summary>
        /// Valida un numero di telefono (formato italiano)
        /// </summary>
        public static ValidationResult ValidatePhoneNumber(string phoneNumber)
        {
            ValidationResult result = new();

            if(string.IsNullOrWhiteSpace(phoneNumber))
            {
                result.AddError("Il numero di telefono è obbligatorio");
                return result;
            }

            // Rimuovi spazi e caratteri speciali
            string cleanedNumber = Regex.Replace(phoneNumber, @"[\s\-\(\)]+", "");

            // Verifica formato italiano
            if(!Regex.IsMatch(cleanedNumber, @"^(\+39)?[0-9]{9,11}$"))
            {
                result.AddError("Il numero di telefono non è valido");
            }

            return result;
        }

        /// <summary>
        /// Combina più risultati di validazione
        /// </summary>
        public static ValidationResult Combine(params ValidationResult[] results)
        {
            ValidationResult combined = new();

            foreach(ValidationResult result in results)
            {
                foreach(string error in result.Errors)
                {
                    combined.AddError(error);
                }
            }

            return combined;
        }
    }

    /// <summary>
    /// Classe per i risultati di validazione
    /// </summary>
    public class ValidationResult
    {
        public List<string> Errors { get; } = [];
        public bool IsValid => !Errors.Any();

        public void AddError(string error)
        {
            if(!string.IsNullOrWhiteSpace(error) && !Errors.Contains(error))
            {
                Errors.Add(error);
            }
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            foreach(string error in errors)
            {
                AddError(error);
            }
        }

        public string GetErrorsAsString(string separator = "\n") => string.Join(separator, Errors);

        public string GetFirstError() => Errors.FirstOrDefault() ?? string.Empty;

        public void Clear() => Errors.Clear();
    }
}