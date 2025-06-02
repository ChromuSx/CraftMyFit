namespace CraftMyFit.Services.Interfaces
{
    public interface IPreferenceService
    {
        void Set<T>(string key, T value);
        T Get<T>(string key, T defaultValue);
        bool ContainsKey(string key);
        void Remove(string key);
        void Clear();

        // Metodi specifici per tipi comuni
        void SetString(string key, string value);
        string GetString(string key, string defaultValue = "");
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue = 0);
        void SetBool(string key, bool value);
        bool GetBool(string key, bool defaultValue = false);
        void SetDateTime(string key, DateTime value);
        DateTime GetDateTime(string key, DateTime defaultValue = default);
    }
}