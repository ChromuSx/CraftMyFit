namespace CraftMyFit.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> ReadTextAsync(string filePath);
        Task WriteTextAsync(string filePath, string content);
        Task<byte[]> ReadBytesAsync(string filePath);
        Task WriteBytesAsync(string filePath, byte[] content);
        Task<bool> ExistsAsync(string filePath);
        Task<bool> DeleteAsync(string filePath);
        Task<bool> CopyAsync(string sourcePath, string destinationPath);
        Task<bool> MoveAsync(string sourcePath, string destinationPath);
        Task<long> GetFileSizeAsync(string filePath);
        Task<DateTime> GetLastModifiedDateAsync(string filePath);
        Task<List<string>> GetFilesInDirectoryAsync(string directoryPath);
        Task<bool> CreateDirectoryAsync(string directoryPath);
        Task<bool> DeleteDirectoryAsync(string directoryPath, bool recursive = false);
        string GetAppDataDirectory();
        string GetDocumentsDirectory();
        string GetCacheDirectory();
        string GetTempDirectory();
        string CombinePath(params string[] paths);
        string GetFileName(string filePath);
        string GetFileNameWithoutExtension(string filePath);
        string GetDirectoryName(string filePath);
        string GetFileExtension(string filePath);
    }
}