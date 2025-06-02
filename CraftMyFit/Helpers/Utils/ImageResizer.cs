namespace CraftMyFit.Helpers.Utils
{
    public static class ImageResizer
    {
        /// <summary>
        /// Ridimensiona un'immagine mantenendo le proporzioni
        /// </summary>
        public static string? ResizeImage(string sourceImagePath, string destinationPath, int maxWidth, int maxHeight, int quality = 85)
        {
            try
            {
                if(!File.Exists(sourceImagePath))
                {
                    return null;
                }

                // Per ora, in assenza di una libreria di image processing,
                // copiamo semplicemente il file originale
                // In un'implementazione completa, useresti SkiaSharp o ImageSharp

                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel ridimensionamento immagine: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea una thumbnail di un'immagine
        /// </summary>
        public static string? CreateThumbnail(string sourceImagePath, string thumbnailPath, int size = 150)
        {
            try
            {
                return ResizeImage(sourceImagePath, thumbnailPath, size, size, 75);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione thumbnail: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ottiene le dimensioni di un'immagine
        /// </summary>
        public static async Task<(int width, int height)?> GetImageDimensionsAsync(string imagePath)
        {
            try
            {
                // In un'implementazione reale, qui leggeresti i metadati dell'immagine
                // Per ora restituiamo dimensioni fittizie
                return await Task.FromResult((1920, 1080));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero dimensioni immagine: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calcola le nuove dimensioni mantenendo le proporzioni
        /// </summary>
        public static (int newWidth, int newHeight) CalculateNewDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
        {
            if(originalWidth <= maxWidth && originalHeight <= maxHeight)
            {
                return (originalWidth, originalHeight);
            }

            double ratioX = (double)maxWidth / originalWidth;
            double ratioY = (double)maxHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            return (newWidth, newHeight);
        }

        /// <summary>
        /// Verifica se un file è un'immagine supportata
        /// </summary>
        public static bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string[] supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            return supportedExtensions.Contains(extension);
        }

        /// <summary>
        /// Ottiene la dimensione di un file in bytes
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            try
            {
                if(!File.Exists(filePath))
                {
                    return 0;
                }

                FileInfo fileInfo = new(filePath);
                return fileInfo.Length;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converte le dimensioni del file in formato leggibile
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
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
        /// Comprime un'immagine riducendone la qualità
        /// </summary>
        public static string? CompressImage(string sourceImagePath, string destinationPath, int quality = 75)
        {
            try
            {
                // In un'implementazione reale, qui comprimeresti l'immagine
                // Per ora copiamo il file originale
                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella compressione immagine: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ruota un'immagine dell'angolo specificato
        /// </summary>
        public static string? RotateImage(string sourceImagePath, string destinationPath, int degrees)
        {
            try
            {
                // In un'implementazione reale, qui ruoteresti l'immagine
                // Per ora copiamo il file originale
                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella rotazione immagine: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converte un'immagine in un formato diverso
        /// </summary>
        public static string? ConvertImageFormat(string sourceImagePath, string destinationPath, string targetFormat = "jpg")
        {
            try
            {
                // In un'implementazione reale, qui convertiresti il formato
                // Per ora copiamo il file originale
                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella conversione formato immagine: {ex.Message}");
                return null;
            }
        }

        #region Metodi di Utilità per Foto di Progresso

        /// <summary>
        /// Prepara una foto di progresso (ridimensiona e ottimizza)
        /// </summary>
        public static string? PrepareProgressPhoto(string sourceImagePath, string destinationDirectory)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(sourceImagePath);
                string destinationPath = Path.Combine(destinationDirectory, $"{fileName}_progress.jpg");

                // Ridimensiona per le foto di progresso (max 1920x1080)
                return ResizeImage(sourceImagePath, destinationPath, 1920, 1080, 85);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella preparazione foto progresso: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea una thumbnail per le foto di progresso
        /// </summary>
        public static async Task<string?> CreateProgressThumbnailAsync(string sourceImagePath, string destinationDirectory)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(sourceImagePath);
                string thumbnailPath = Path.Combine(destinationDirectory, $"{fileName}_thumb.jpg");

                return CreateThumbnail(sourceImagePath, thumbnailPath, 200);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione thumbnail progresso: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}