using CraftMyFit.Services.Interfaces;
using System.Text.Json;

namespace CraftMyFit.Services
{
    public class PreferenceService : IPreferenceService
    {
        public void Set<T>(string key, T value)
        {
            if(value == null)
            {
                Remove(key);
                return;
            }

            try
            {
                string jsonValue = JsonSerializer.Serialize(value);
                Preferences.Set(key, jsonValue);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio della preferenza {key}: {ex.Message}");
                // Fallback per tipi semplici
                if(value is string stringValue)
                {
                    Preferences.Set(key, stringValue);
                }
                else if(value is int intValue)
                {
                    Preferences.Set(key, intValue);
                }
                else if(value is bool boolValue)
                {
                    Preferences.Set(key, boolValue);
                }
                else if(value is DateTime dateTimeValue)
                {
                    Preferences.Set(key, dateTimeValue);
                }
                else
                {
                    throw;
                }
            }
        }

        public T Get<T>(string key, T defaultValue)
        {
            try
            {
                if(!ContainsKey(key))
                {
                    return defaultValue;
                }

                // Gestione diretta per tipi primitivi
                if(typeof(T) == typeof(string))
                {
                    return (T)(object)Preferences.Get(key, (string)(object)defaultValue);
                }

                if(typeof(T) == typeof(int))
                {
                    return (T)(object)Preferences.Get(key, (int)(object)defaultValue);
                }

                if(typeof(T) == typeof(bool))
                {
                    return (T)(object)Preferences.Get(key, (bool)(object)defaultValue);
                }

                if(typeof(T) == typeof(DateTime))
                {
                    return (T)(object)Preferences.Get(key, (DateTime)(object)defaultValue);
                }

                // Per oggetti complessi, usa JSON
                string jsonValue = Preferences.Get(key, string.Empty);
                if(string.IsNullOrEmpty(jsonValue))
                {
                    return defaultValue;
                }

                T result = JsonSerializer.Deserialize<T>(jsonValue);
                return result ?? defaultValue;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero della preferenza {key}: {ex.Message}");
                return defaultValue;
            }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return Preferences.ContainsKey(key);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo esistenza preferenza {key}: {ex.Message}");
                return false;
            }
        }

        public void Remove(string key)
        {
            try
            {
                Preferences.Remove(key);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella rimozione della preferenza {key}: {ex.Message}");
            }
        }

        public void Clear()
        {
            try
            {
                Preferences.Clear();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella pulizia delle preferenze: {ex.Message}");
            }
        }

        // Metodi specifici per tipi comuni
        public void SetString(string key, string value)
        {
            try
            {
                Preferences.Set(key, value);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio stringa {key}: {ex.Message}");
            }
        }

        public string GetString(string key, string defaultValue = "")
        {
            try
            {
                return Preferences.Get(key, defaultValue);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero stringa {key}: {ex.Message}");
                return defaultValue;
            }
        }

        public void SetInt(string key, int value)
        {
            try
            {
                Preferences.Set(key, value);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio intero {key}: {ex.Message}");
            }
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            try
            {
                return Preferences.Get(key, defaultValue);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero intero {key}: {ex.Message}");
                return defaultValue;
            }
        }

        public void SetBool(string key, bool value)
        {
            try
            {
                Preferences.Set(key, value);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio boolean {key}: {ex.Message}");
            }
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            try
            {
                return Preferences.Get(key, defaultValue);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero boolean {key}: {ex.Message}");
                return defaultValue;
            }
        }

        public void SetDateTime(string key, DateTime value)
        {
            try
            {
                Preferences.Set(key, value);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio DateTime {key}: {ex.Message}");
            }
        }

        public DateTime GetDateTime(string key, DateTime defaultValue = default)
        {
            try
            {
                return Preferences.Get(key, defaultValue);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero DateTime {key}: {ex.Message}");
                return defaultValue;
            }
        }

        #region Metodi di utilità

        /// <summary>
        /// Salva un oggetto complesso serializzandolo in JSON
        /// </summary>
        public void SetObject<T>(string key, T value) => Set(key, value);

        /// <summary>
        /// Recupera un oggetto complesso deserializzandolo da JSON
        /// </summary>
        public T GetObject<T>(string key, T defaultValue = default) => Get(key, defaultValue);

        /// <summary>
        /// Esporta tutte le preferenze in un dizionario
        /// </summary>
        public Dictionary<string, object> ExportAll() =>
            // Nota: Microsoft.Maui.Essentials.Preferences non fornisce un modo per 
            // enumerare tutte le chiavi, quindi questo metodo è limitato
            // Potremmo mantenere un registro delle chiavi utilizzate
            [];

        /// <summary>
        /// Importa preferenze da un dizionario
        /// </summary>
        public void ImportAll(Dictionary<string, object> preferences)
        {
            foreach(KeyValuePair<string, object> kvp in preferences)
            {
                try
                {
                    Set(kvp.Key, kvp.Value);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Errore nell'importazione preferenza {kvp.Key}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Ottiene la dimensione approssimativa delle preferenze salvate
        /// </summary>
        public long GetApproximateSize() =>
            // Implementazione limitata - calcolo approssimativo
            // In un'implementazione reale potresti mantenere statistiche
            0;

        #endregion
    }
}