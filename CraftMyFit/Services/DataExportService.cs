using CraftMyFit.Data.Interfaces;
using CraftMyFit.Services.Interfaces;
using System.Text.Json;
using System.Text;

namespace CraftMyFit.Services
{
    public class DataExportService : IDataExportService
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IBodyMeasurementRepository _bodyMeasurementRepository;
        private readonly IProgressPhotoRepository _progressPhotoRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly StatsService _statsService;
        private readonly IFileService _fileService;

        public DataExportService(
            IWorkoutPlanRepository workoutPlanRepository,
            IBodyMeasurementRepository bodyMeasurementRepository,
            IProgressPhotoRepository progressPhotoRepository,
            IAchievementRepository achievementRepository,
            StatsService statsService,
            IFileService fileService)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _bodyMeasurementRepository = bodyMeasurementRepository;
            _progressPhotoRepository = progressPhotoRepository;
            _achievementRepository = achievementRepository;
            _statsService = statsService;
            _fileService = fileService;
        }

        public async Task<string?> ExportAllDataAsJsonAsync(int userId)
        {
            try
            {
                var exportData = new
                {
                    ExportInfo = new
                    {
                        UserId = userId,
                        ExportDate = DateTime.Now,
                        AppVersion = AppInfo.VersionString,
                        DataVersion = "1.0"
                    },
                    WorkoutPlans = await _workoutPlanRepository.GetByUserIdAsync(userId),
                    BodyMeasurements = await _bodyMeasurementRepository.GetByUserIdAsync(userId),
                    ProgressPhotos = await _progressPhotoRepository.GetByUserIdAsync(userId),
                    Achievements = await _achievementRepository.GetByUserIdAsync(userId),
                    Stats = await _statsService.CalculateUserStatsAsync(userId)
                };

                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fileName = $"CraftMyFit_Export_{userId}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, json);
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione JSON: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ExportAllDataAsCsvAsync(int userId)
        {
            try
            {
                var csvBuilder = new StringBuilder();

                // Header del file
                csvBuilder.AppendLine("CraftMyFit Data Export");
                csvBuilder.AppendLine($"User ID: {userId}");
                csvBuilder.AppendLine($"Export Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                csvBuilder.AppendLine();

                // Esporta misurazioni corporee
                csvBuilder.AppendLine("=== BODY MEASUREMENTS ===");
                var measurements = await _bodyMeasurementRepository.GetByUserIdAsync(userId);
                csvBuilder.AppendLine("Date,Weight,BodyFat,MuscleMass,Chest,Waist,Hips,LeftArm,RightArm,LeftThigh,RightThigh");

                foreach(var measurement in measurements)
                {
                    csvBuilder.AppendLine($"{measurement.Date:yyyy-MM-dd}," +
                                        $"{measurement.Weight}," +
                                        $"{measurement.BodyFatPercentage}," +
                                        $"{measurement.MuscleMass}," +
                                        $"{measurement.Chest}," +
                                        $"{measurement.Waist}," +
                                        $"{measurement.Hips}," +
                                        $"{measurement.LeftArm}," +
                                        $"{measurement.RightArm}," +
                                        $"{measurement.LeftThigh}," +
                                        $"{measurement.RightThigh}");
                }

                csvBuilder.AppendLine();

                // Esporta foto di progresso (solo metadati)
                csvBuilder.AppendLine("=== PROGRESS PHOTOS ===");
                var photos = await _progressPhotoRepository.GetByUserIdAsync(userId);
                csvBuilder.AppendLine("Date,Description,FilePath");

                foreach(var photo in photos)
                {
                    csvBuilder.AppendLine($"{photo.Date:yyyy-MM-dd}," +
                                        $"\"{photo.Description}\"," +
                                        $"\"{photo.PhotoPath}\"");
                }

                var fileName = $"CraftMyFit_Export_{userId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, csvBuilder.ToString());
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione CSV: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ExportBodyMeasurementsAsync(int userId)
        {
            try
            {
                var measurements = await _bodyMeasurementRepository.GetByUserIdAsync(userId);

                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Date,Weight,BodyFatPercentage,MuscleMass,Chest,Waist,Hips,LeftArm,RightArm,LeftThigh,RightThigh,Notes");

                foreach(var measurement in measurements)
                {
                    csvBuilder.AppendLine($"{measurement.Date:yyyy-MM-dd}," +
                                        $"{measurement.Weight}," +
                                        $"{measurement.BodyFatPercentage}," +
                                        $"{measurement.MuscleMass}," +
                                        $"{measurement.Chest}," +
                                        $"{measurement.Waist}," +
                                        $"{measurement.Hips}," +
                                        $"{measurement.LeftArm}," +
                                        $"{measurement.RightArm}," +
                                        $"{measurement.LeftThigh}," +
                                        $"{measurement.RightThigh}," +
                                        $"\"{measurement.Notes}\"");
                }

                var fileName = $"BodyMeasurements_{userId}_{DateTime.Now:yyyyMMdd}.csv";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, csvBuilder.ToString());
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione misurazioni: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ExportWorkoutPlansAsync(int userId)
        {
            try
            {
                var workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(userId);

                var json = JsonSerializer.Serialize(workoutPlans, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fileName = $"WorkoutPlans_{userId}_{DateTime.Now:yyyyMMdd}.json";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, json);
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione piani: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ExportProgressPhotosMetadataAsync(int userId)
        {
            try
            {
                var photos = await _progressPhotoRepository.GetByUserIdAsync(userId);

                var photoMetadata = photos.Select(p => new
                {
                    p.Id,
                    p.Date,
                    p.Description,
                    p.PhotoPath,
                    FileExists = File.Exists(p.PhotoPath),
                    FileSize = File.Exists(p.PhotoPath) ? new FileInfo(p.PhotoPath).Length : 0
                });

                var json = JsonSerializer.Serialize(photoMetadata, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fileName = $"ProgressPhotos_Metadata_{userId}_{DateTime.Now:yyyyMMdd}.json";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, json);
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione metadata foto: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ExportAchievementsAsync(int userId)
        {
            try
            {
                var achievements = await _achievementRepository.GetByUserIdAsync(userId);

                var json = JsonSerializer.Serialize(achievements, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fileName = $"Achievements_{userId}_{DateTime.Now:yyyyMMdd}.json";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, json);
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione achievement: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ExportStatsReportAsync(int userId)
        {
            try
            {
                var stats = await _statsService.CalculateUserStatsAsync(userId);

                var reportBuilder = new StringBuilder();
                reportBuilder.AppendLine("=== CRAFTMYFIT STATS REPORT ===");
                reportBuilder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                reportBuilder.AppendLine($"User ID: {userId}");
                reportBuilder.AppendLine();

                reportBuilder.AppendLine("WORKOUT STATISTICS:");
                reportBuilder.AppendLine($"- Total Workout Plans: {stats.TotalWorkoutPlans}");
                reportBuilder.AppendLine($"- Active Workout Plans: {stats.ActiveWorkoutPlans}");
                reportBuilder.AppendLine($"- Completed Workouts: {stats.CompletedWorkouts}");
                reportBuilder.AppendLine($"- Current Streak: {stats.CurrentStreak} days");
                reportBuilder.AppendLine($"- Longest Streak: {stats.LongestStreak} days");
                reportBuilder.AppendLine();

                reportBuilder.AppendLine("PROGRESS STATISTICS:");
                reportBuilder.AppendLine($"- Total Measurements: {stats.TotalMeasurements}");
                reportBuilder.AppendLine($"- Total Progress Photos: {stats.TotalProgressPhotos}");
                reportBuilder.AppendLine($"- Current Weight: {stats.CurrentWeight} kg");
                reportBuilder.AppendLine($"- Weight Change: {stats.WeightChangeText}");
                reportBuilder.AppendLine();

                reportBuilder.AppendLine("ACHIEVEMENT STATISTICS:");
                reportBuilder.AppendLine($"- Total Achievements: {stats.TotalAchievements}");
                reportBuilder.AppendLine($"- Unlocked Achievements: {stats.UnlockedAchievements}");
                reportBuilder.AppendLine($"- Completion Rate: {stats.AchievementCompletionRate:F1}%");
                reportBuilder.AppendLine($"- Total Points: {stats.TotalPoints}");

                var fileName = $"StatsReport_{userId}_{DateTime.Now:yyyyMMdd}.txt";
                var filePath = Path.Combine(_fileService.GetDocumentsDirectory(), fileName);

                await _fileService.WriteTextAsync(filePath, reportBuilder.ToString());
                return filePath;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'esportazione report stats: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ImportDataFromJsonAsync(int userId, string jsonFilePath)
        {
            try
            {
                if(!await _fileService.ExistsAsync(jsonFilePath))
                {
                    return false;
                }

                var jsonContent = await _fileService.ReadTextAsync(jsonFilePath);
                if(string.IsNullOrEmpty(jsonContent))
                {
                    return false;
                }

                // TODO: Implementare l'importazione dei dati
                // Questo richiederebbe la validazione e l'inserimento sicuro dei dati nel database

                await Task.Delay(100); // Placeholder
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'importazione JSON: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> CreatePdfReportAsync(int userId)
        {
            try
            {
                // In un'implementazione reale, qui useresti una libreria PDF
                // come QuestPDF o iTextSharp per creare un report PDF completo

                await Task.Delay(100); // Placeholder
                return null; // Non implementato
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione PDF: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ValidateExportedDataAsync(string filePath)
        {
            try
            {
                if(!await _fileService.ExistsAsync(filePath))
                {
                    return false;
                }

                var fileSize = await _fileService.GetFileSizeAsync(filePath);
                if(fileSize == 0)
                {
                    return false;
                }

                // Validazioni aggiuntive basate sul tipo di file
                var extension = Path.GetExtension(filePath).ToLower();

                switch(extension)
                {
                    case ".json":
                        return await ValidateJsonFileAsync(filePath);
                    case ".csv":
                        return await ValidateCsvFileAsync(filePath);
                    default:
                        return true; // File sconosciuto, assumiamo sia valido
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella validazione file: {ex.Message}");
                return false;
            }
        }

        #region Metodi privati

        private async Task<bool> ValidateJsonFileAsync(string filePath)
        {
            try
            {
                var content = await _fileService.ReadTextAsync(filePath);
                JsonDocument.Parse(content);
                return true;
            }
            catch(JsonException)
            {
                return false;
            }
        }

        private async Task<bool> ValidateCsvFileAsync(string filePath)
        {
            try
            {
                var content = await _fileService.ReadTextAsync(filePath);
                var lines = content.Split('\n');
                return lines.Length > 1; // Almeno header + 1 riga di dati
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}