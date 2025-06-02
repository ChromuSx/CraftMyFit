namespace CraftMyFit.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string?> TakePhotoAsync();
        Task<string?> PickPhotoAsync();
        Task<string?> SavePhotoAsync(Stream photoStream, string fileName);
        Task<bool> DeletePhotoAsync(string filePath);
        Task<Stream?> LoadPhotoAsync(string filePath);
        Task<string?> ResizePhotoAsync(string sourceFilePath, int maxWidth, int maxHeight, int quality = 85);
        Task<byte[]?> GetPhotoThumbnailAsync(string filePath, int width = 150, int height = 150);
        string GetPhotosDirectory();
        string GetProgressPhotosDirectory();
        bool PhotoExists(string filePath);
        long GetPhotoFileSize(string filePath);
        string GenerateUniqueFileName(string extension = ".jpg");
    }
}