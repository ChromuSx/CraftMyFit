using CraftMyFit.Services.Interfaces;

namespace CraftMyFit.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly string _photosDirectory;
        private readonly string _progressPhotosDirectory;

        public PhotoService()
        {
            _photosDirectory = Path.Combine(FileSystem.AppDataDirectory, "Photos");
            _progressPhotosDirectory = Path.Combine(_photosDirectory, "Progress");

            // Crea le directory se non esistono
            _ = Directory.CreateDirectory(_photosDirectory);
            _ = Directory.CreateDirectory(_progressPhotosDirectory);
        }

        public async Task<string?> TakePhotoAsync()
        {
            try
            {
                if(!MediaPicker.Default.IsCaptureSupported)
                {
                    return null;
                }

                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
                return photo == null ? null : await ProcessAndSavePhotoAsync(photo);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel scattare la foto: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> PickPhotoAsync()
        {
            try
            {
                FileResult? photo = await MediaPicker.Default.PickPhotoAsync();
                return photo == null ? null : await ProcessAndSavePhotoAsync(photo);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella selezione della foto: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> SavePhotoAsync(Stream photoStream, string fileName)
        {
            try
            {
                string filePath = Path.Combine(_progressPhotosDirectory, fileName);

                using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
                await photoStream.CopyToAsync(fileStream);

                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio della foto: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeletePhotoAsync(string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if(File.Exists(filePath))
                    {
                        File.Delete(filePath);

                        // Elimina anche la thumbnail se esiste
                        string thumbnailPath = GetThumbnailPath(filePath);
                        if(File.Exists(thumbnailPath))
                        {
                            File.Delete(thumbnailPath);
                        }

                        return true;
                    }

                    return false;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'eliminazione della foto: {ex.Message}");
                return false;
            }
        }

        public async Task<Stream?> LoadPhotoAsync(string filePath)
        {
            try
            {
                return await Task.Run(() => !File.Exists(filePath) ? null : (Stream)File.OpenRead(filePath));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento della foto: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ResizePhotoAsync(string sourceFilePath, int maxWidth, int maxHeight, int quality = 85)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if(!File.Exists(sourceFilePath))
                    {
                        return null;
                    }

                    // Per ora restituiamo il path originale
                    // In un'implementazione completa, qui useremo una libreria di image processing
                    // come SkiaSharp o ImageSharp per ridimensionare l'immagine

                    // TODO: Implementare il ridimensionamento effettivo
                    return sourceFilePath;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel ridimensionamento della foto: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> GetPhotoThumbnailAsync(string filePath, int width = 150, int height = 150)
        {
            try
            {
                string thumbnailPath = GetThumbnailPath(filePath);

                // Se la thumbnail esiste già, restituiscila
                if(File.Exists(thumbnailPath))
                {
                    return await File.ReadAllBytesAsync(thumbnailPath);
                }

                // Altrimenti crea la thumbnail
                // TODO: Implementare la creazione di thumbnail
                // Per ora restituiamo l'immagine originale ridimensionata
                return File.Exists(filePath) ? await File.ReadAllBytesAsync(filePath) : null;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione della thumbnail: {ex.Message}");
                return null;
            }
        }

        public string GetPhotosDirectory() => _photosDirectory;

        public string GetProgressPhotosDirectory() => _progressPhotosDirectory;

        public bool PhotoExists(string filePath) => File.Exists(filePath);

        public long GetPhotoFileSize(string filePath)
        {
            try
            {
                if(File.Exists(filePath))
                {
                    FileInfo fileInfo = new(filePath);
                    return fileInfo.Length;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public string GenerateUniqueFileName(string extension = ".jpg")
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string guid = Guid.NewGuid().ToString("N")[..8];
            return $"photo_{timestamp}_{guid}{extension}";
        }

        private async Task<string?> ProcessAndSavePhotoAsync(FileResult photo)
        {
            try
            {
                string fileName = GenerateUniqueFileName();
                string filePath = Path.Combine(_progressPhotosDirectory, fileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
                await sourceStream.CopyToAsync(fileStream);

                // Opzionalmente ridimensiona l'immagine
                // var resizedPath = await ResizePhotoAsync(filePath, 1920, 1080, 85);

                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel processare la foto: {ex.Message}");
                return null;
            }
        }

        private string GetThumbnailPath(string originalPath)
        {
            string? directory = Path.GetDirectoryName(originalPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);

            string thumbnailDirectory = Path.Combine(directory!, "thumbnails");
            _ = Directory.CreateDirectory(thumbnailDirectory);

            return Path.Combine(thumbnailDirectory, $"{fileNameWithoutExtension}_thumb{extension}");
        }

        #region Metodi aggiuntivi per funzionalità avanzate

        /// <summary>
        /// Comprime una foto riducendo la qualità
        /// </summary>
        public async Task<string?> CompressPhotoAsync(string sourceFilePath, int quality = 75)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if(!File.Exists(sourceFilePath))
                    {
                        return null;
                    }

                    // TODO: Implementare compressione effettiva
                    // Per ora restituiamo il path originale
                    return sourceFilePath;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella compressione foto: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ruota una foto dell'angolo specificato
        /// </summary>
        public async Task<string?> RotatePhotoAsync(string sourceFilePath, int degrees)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if(!File.Exists(sourceFilePath))
                    {
                        return null;
                    }

                    // TODO: Implementare rotazione effettiva
                    // Per ora restituiamo il path originale
                    return sourceFilePath;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella rotazione foto: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ottiene i metadati EXIF di una foto
        /// </summary>
        public async Task<Dictionary<string, string>> GetPhotoMetadataAsync(string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    Dictionary<string, string> metadata = [];

                    if(!File.Exists(filePath))
                    {
                        return metadata;
                    }

                    FileInfo fileInfo = new(filePath);
                    metadata["FileName"] = fileInfo.Name;
                    metadata["FileSize"] = fileInfo.Length.ToString();
                    metadata["CreationTime"] = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                    metadata["LastWriteTime"] = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

                    // TODO: Aggiungere lettura metadati EXIF reali

                    return metadata;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero metadati foto: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Pulisce le foto orfane e le thumbnail non utilizzate
        /// </summary>
        public async Task<int> CleanupOrphanedPhotosAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    int deletedCount = 0;

                    // Pulisci le thumbnail orfane
                    string thumbnailsDir = Path.Combine(_progressPhotosDirectory, "thumbnails");
                    if(Directory.Exists(thumbnailsDir))
                    {
                        string[] thumbnails = Directory.GetFiles(thumbnailsDir);
                        foreach(string thumbnailPath in thumbnails)
                        {
                            string originalName = Path.GetFileNameWithoutExtension(thumbnailPath).Replace("_thumb", "");
                            string extension = Path.GetExtension(thumbnailPath);
                            string originalPath = Path.Combine(_progressPhotosDirectory, $"{originalName}{extension}");

                            if(!File.Exists(originalPath))
                            {
                                File.Delete(thumbnailPath);
                                deletedCount++;
                            }
                        }
                    }

                    return deletedCount;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella pulizia foto orfane: {ex.Message}");
                return 0;
            }
        }

        #endregion
    }
}