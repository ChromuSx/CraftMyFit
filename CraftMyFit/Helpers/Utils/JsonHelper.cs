using System.Text.Json;

namespace CraftMyFit.Helpers.Utils
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions PrettyOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Serializza un oggetto in JSON
        /// </summary>
        public static string Serialize<T>(T obj, bool pretty = false)
        {
            try
            {
                JsonSerializerOptions options = pretty ? PrettyOptions : DefaultOptions;
                return JsonSerializer.Serialize(obj, options);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Errore nella serializzazione JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deserializza una stringa JSON in un oggetto
        /// </summary>
        public static T? Deserialize<T>(string json)
        {
            if(string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(json, DefaultOptions);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Errore nella deserializzazione JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deserializza una stringa JSON in un oggetto con valore di default
        /// </summary>
        public static T DeserializeOrDefault<T>(string json, T defaultValue)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(json))
                {
                    return defaultValue;
                }

                T? result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
                return result ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Verifica se una stringa è JSON valido
        /// </summary>
        public static bool IsValidJson(string json)
        {
            if(string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clona un oggetto tramite serializzazione/deserializzazione JSON
        /// </summary>
        public static T? DeepClone<T>(T obj)
        {
            try
            {
                string json = Serialize(obj);
                return Deserialize<T>(json);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Errore nel clone dell'oggetto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converte un oggetto in un Dictionary per il binding
        /// </summary>
        public static Dictionary<string, object?> ToValueDictionary<T>(T obj)
        {
            try
            {
                string json = Serialize(obj);
                Dictionary<string, JsonElement>? dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, DefaultOptions);

                Dictionary<string, object?> result = new();

                if(dictionary != null)
                {
                    foreach(KeyValuePair<string, JsonElement> kvp in dictionary)
                    {
                        result[kvp.Key] = JsonElementToObject(kvp.Value);
                    }
                }

                return result;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Errore nella conversione a Dictionary: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Formatta una stringa JSON per la visualizzazione
        /// </summary>
        public static string FormatJson(string json)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(json))
                {
                    return string.Empty;
                }

                using JsonDocument document = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(document, PrettyOptions);
            }
            catch
            {
                return json; // Restituisce la stringa originale se non è JSON valido
            }
        }

        /// <summary>
        /// Unisce due oggetti JSON (il secondo sovrascrive il primo)
        /// </summary>
        public static T MergeObjects<T>(T first, T second)
        {
            try
            {
                Dictionary<string, object?> firstDict = ToValueDictionary(first);
                Dictionary<string, object?> secondDict = ToValueDictionary(second);

                foreach(KeyValuePair<string, object?> kvp in secondDict)
                {
                    firstDict[kvp.Key] = kvp.Value;
                }

                string mergedJson = Serialize(firstDict);
                return Deserialize<T>(mergedJson) ?? first;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Errore nella fusione degli oggetti: {ex.Message}", ex);
            }
        }

        private static object? JsonElementToObject(JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt32(out int intValue) ? intValue : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => element.EnumerateArray().Select(JsonElementToObject).ToArray(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => JsonElementToObject(p.Value)),
            _ => element.ToString()
        };
    }
}