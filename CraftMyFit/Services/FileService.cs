using CraftMyFit.Services.Interfaces;

namespace CraftMyFit.Services
{
    public class FileService : IFileService
    {
        public async Task<string> ReadTextAsync(string filePath)
        {
            try
            {
                return !File.Exists(filePath) ? string.Empty : await File.ReadAllTextAsync(filePath);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella lettura del file {filePath}: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task WriteTextAsync(string filePath, string content)
        {
            try
            {
                // Crea la directory se non esiste
                string? directory = Path.GetDirectoryName(filePath);
                if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    _ = Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, content);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella scrittura del file {filePath}: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> ReadBytesAsync(string filePath)
        {
            try
            {
                return !File.Exists(filePath) ? Array.Empty<byte>() : await File.ReadAllBytesAsync(filePath);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella lettura bytes del file {filePath}: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public async Task WriteBytesAsync(string filePath, byte[] content)
        {
            try
            {
                // Crea la directory se non esiste
                string? directory = Path.GetDirectoryName(filePath);
                if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    _ = Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(filePath, content);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella scrittura bytes del file {filePath}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string filePath)
        {
            try
            {
                return await Task.FromResult(File.Exists(filePath));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo esistenza file {filePath}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string filePath)
        {
            try
            {
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'eliminazione file {filePath}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CopyAsync(string sourcePath, string destinationPath)
        {
            try
            {
                if(!File.Exists(sourcePath))
                {
                    return false;
                }

                // Crea la directory di destinazione se non esiste
                string? directory = Path.GetDirectoryName(destinationPath);
                if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    _ = Directory.CreateDirectory(directory);
                }

                File.Copy(sourcePath, destinationPath, true);
                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella copia da {sourcePath} a {destinationPath}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MoveAsync(string sourcePath, string destinationPath)
        {
            try
            {
                if(!File.Exists(sourcePath))
                {
                    return false;
                }

                // Crea la directory di destinazione se non esiste
                string? directory = Path.GetDirectoryName(destinationPath);
                if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    _ = Directory.CreateDirectory(directory);
                }

                File.Move(sourcePath, destinationPath);
                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nello spostamento da {sourcePath} a {destinationPath}: {ex.Message}");
                return false;
            }
        }

        public async Task<long> GetFileSizeAsync(string filePath)
        {
            try
            {
                if(!File.Exists(filePath))
                {
                    return 0;
                }

                FileInfo fileInfo = new(filePath);
                return await Task.FromResult(fileInfo.Length);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel calcolo dimensione file {filePath}: {ex.Message}");
                return 0;
            }
        }

        public async Task<DateTime> GetLastModifiedDateAsync(string filePath)
        {
            try
            {
                if(!File.Exists(filePath))
                {
                    return DateTime.MinValue;
                }

                FileInfo fileInfo = new(filePath);
                return await Task.FromResult(fileInfo.LastWriteTime);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero data modifica file {filePath}: {ex.Message}");
                return DateTime.MinValue;
            }
        }

        public async Task<List<string>> GetFilesInDirectoryAsync(string directoryPath)
        {
            try
            {
                if(!Directory.Exists(directoryPath))
                {
                    return [];
                }

                string[] files = Directory.GetFiles(directoryPath);
                return await Task.FromResult(files.ToList());
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero file dalla directory {directoryPath}: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> CreateDirectoryAsync(string directoryPath)
        {
            try
            {
                if(!Directory.Exists(directoryPath))
                {
                    _ = Directory.CreateDirectory(directoryPath);
                }

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione directory {directoryPath}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDirectoryAsync(string directoryPath, bool recursive = false)
        {
            try
            {
                if(Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, recursive);
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'eliminazione directory {directoryPath}: {ex.Message}");
                return false;
            }
        }

        public string GetAppDataDirectory() => FileSystem.AppDataDirectory;

        public string GetDocumentsDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string GetCacheDirectory() => FileSystem.CacheDirectory;

        public string GetTempDirectory() => Path.GetTempPath();

        public string CombinePath(params string[] paths) => paths == null || paths.Length == 0 ? string.Empty : Path.Combine(paths);

        public string GetFileName(string filePath) => Path.GetFileName(filePath);

        public string GetFileNameWithoutExtension(string filePath) => Path.GetFileNameWithoutExtension(filePath);

        public string GetDirectoryName(string filePath) => Path.GetDirectoryName(filePath) ?? string.Empty;

        public string GetFileExtension(string filePath) => Path.GetExtension(filePath);

        #region Metodi di Utilità

        /// <summary>
        /// Formatta la dimensione del file in modo leggibile
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while(len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Verifica se l'estensione del file è supportata per le immagini
        /// </summary>
        public static bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            return imageExtensions.Contains(extension);
        }

        /// <summary>
        /// Genera un nome file unico aggiungendo un numero se necessario
        /// </summary>
        public async Task<string> GenerateUniqueFileNameAsync(string basePath, string fileName)
        {
            string fullPath = Path.Combine(basePath, fileName);

            if(!await ExistsAsync(fullPath))
            {
                return fullPath;
            }

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            int counter = 1;

            while(true)
            {
                string newFileName = $"{nameWithoutExtension}_{counter}{extension}";
                string newFullPath = Path.Combine(basePath, newFileName);

                if(!await ExistsAsync(newFullPath))
                {
                    return newFullPath;
                }

                counter++;
            }
        }

        /// <summary>
        /// Pulisce il nome del file da caratteri non validi
        /// </summary>
        public static string SanitizeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = fileName;

            foreach(char invalidChar in invalidChars)
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }

            return sanitized;
        }

        #endregion
    }
}