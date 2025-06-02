using CraftMyFit.Data.Interfaces;
using CraftMyFit.Services.Interfaces;

namespace CraftMyFit.Services
{
    public class SharingService : ISharingService
    {
        private readonly IProgressPhotoRepository _progressPhotoRepository;
        private readonly IBodyMeasurementRepository _bodyMeasurementRepository;
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly StatsService _statsService;

        public SharingService(
            IProgressPhotoRepository progressPhotoRepository,
            IBodyMeasurementRepository bodyMeasurementRepository,
            IWorkoutPlanRepository workoutPlanRepository,
            IAchievementRepository achievementRepository,
            StatsService statsService)
        {
            _progressPhotoRepository = progressPhotoRepository;
            _bodyMeasurementRepository = bodyMeasurementRepository;
            _workoutPlanRepository = workoutPlanRepository;
            _achievementRepository = achievementRepository;
            _statsService = statsService;
        }

        public async Task<bool> ShareTextAsync(string title, string text)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = text,
                    Title = title
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione testo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareImageAsync(string title, string imagePath, string? text = null)
        {
            try
            {
                if(!File.Exists(imagePath))
                {
                    return false;
                }

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(imagePath)
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione immagine: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareImagesAsync(string title, List<string> imagePaths, string? text = null)
        {
            try
            {
                List<ShareFile> files = imagePaths
                    .Where(File.Exists)
                    .Select(path => new ShareFile(path))
                    .ToList();

                if(!files.Any())
                {
                    return false;
                }

                await Share.Default.RequestAsync(new ShareMultipleFilesRequest
                {
                    Title = title,
                    Files = files
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione immagini: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareFileAsync(string title, string filePath, string? text = null)
        {
            try
            {
                if(!File.Exists(filePath))
                {
                    return false;
                }

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(filePath)
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione file: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareProgressAsync(string userId, ShareProgressType type)
        {
            try
            {
                int userIdInt = int.Parse(userId);
                string progressText = await GenerateProgressTextAsync(userIdInt, type);

                if(string.IsNullOrEmpty(progressText))
                {
                    return false;
                }

                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = progressText,
                    Title = $"Il mio progresso con CraftMyFit 💪"
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione progresso: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareWorkoutPlanAsync(string workoutPlanId)
        {
            try
            {
                int planId = int.Parse(workoutPlanId);
                Models.Workout.WorkoutPlan workoutPlan = await _workoutPlanRepository.GetWorkoutPlanWithDetailsAsync(planId);

                if(workoutPlan == null)
                {
                    return false;
                }

                string shareText = GenerateWorkoutPlanShareText(workoutPlan);

                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = $"Piano di Allenamento: {workoutPlan.Title}"
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione piano: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareAchievementAsync(string achievementId)
        {
            try
            {
                int achId = int.Parse(achievementId);
                Models.Gamification.Achievement achievement = await _achievementRepository.GetByIdAsync(achId);

                if(achievement == null || !achievement.UnlockedDate.HasValue)
                {
                    return false;
                }

                string shareText = $"🏆 Achievement sbloccato su CraftMyFit!\n\n" +
                              $"✨ {achievement.Title}\n" +
                              $"📝 {achievement.Description}\n" +
                              $"🎯 {achievement.PointsAwarded} punti guadagnati\n\n" +
                              $"#CraftMyFit #Fitness #Achievement #Motivazione";

                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = "Achievement Sbloccato! 🏆"
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione achievement: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareCustomStatsAsync(Dictionary<string, object> stats)
        {
            try
            {
                string shareText = "📊 Le mie statistiche CraftMyFit:\n\n";

                foreach(KeyValuePair<string, object> stat in stats)
                {
                    shareText += $"• {stat.Key}: {stat.Value}\n";
                }

                shareText += "\n💪 Raggiungi i tuoi obiettivi con CraftMyFit!\n#CraftMyFit #Fitness #Stats";

                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = "Le mie statistiche fitness 📊"
                });
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione stats: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareStatsImageAsync(string userId)
        {
            try
            {
                // Crea un'immagine con le statistiche dell'utente
                int userIdInt = int.Parse(userId);
                string? imagePath = await CreateStatsImageAsync(userIdInt);

                return !string.IsNullOrEmpty(imagePath) && await ShareImageAsync("Le mie statistiche CraftMyFit 📊", imagePath,
                    "Guarda i miei progressi fitness! 💪 #CraftMyFit #Fitness");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella condivisione immagine stats: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsSharingAvailableAsync()
        {
            try
            {
                return await Task.FromResult(Share.Default != null);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo disponibilità condivisione: {ex.Message}");
                return false;
            }
        }

        #region Metodi privati

        private async Task<string> GenerateProgressTextAsync(int userId, ShareProgressType type)
        {
            try
            {
                UserStats stats = await _statsService.CalculateUserStatsAsync(userId);

                return type switch
                {
                    ShareProgressType.WeightLoss => GenerateWeightLossText(stats),
                    ShareProgressType.MuscleGain => GenerateMuscleGainText(stats),
                    ShareProgressType.WorkoutStreak => GenerateWorkoutStreakText(stats),
                    ShareProgressType.PhotoComparison => await GeneratePhotoComparisonTextAsync(userId),
                    ShareProgressType.MonthlyProgress => GenerateMonthlyProgressText(stats),
                    ShareProgressType.YearlyProgress => GenerateYearlyProgressText(stats),
                    _ => GenerateGeneralProgressText(stats)
                };
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella generazione testo progresso: {ex.Message}");
                return string.Empty;
            }
        }

        private string GenerateWeightLossText(UserStats stats)
        {
            if(stats.WeightChange >= 0)
            {
                return string.Empty; // Nessuna perdita di peso da condividere
            }

            return $"🎉 Ho perso {Math.Abs(stats.WeightChange.Value):F1} kg con CraftMyFit!\n\n" +
                   $"📊 I miei risultati:\n" +
                   $"• Peso attuale: {stats.CurrentWeight:F1} kg\n" +
                   $"• Allenamenti completati: {stats.CompletedWorkouts}\n" +
                   $"• Streak corrente: {stats.CurrentStreak} giorni\n\n" +
                   $"💪 La costanza paga sempre!\n" +
                   $"#CraftMyFit #WeightLoss #Fitness #Motivazione";
        }

        private string GenerateMuscleGainText(UserStats stats) => $"💪 Sto costruendo il mio fisico con CraftMyFit!\n\n" +
                   $"📈 I miei progressi:\n" +
                   $"• Allenamenti completati: {stats.CompletedWorkouts}\n" +
                   $"• Streak più lunga: {stats.LongestStreak} giorni\n" +
                   $"• Achievement sbloccati: {stats.UnlockedAchievements}/{stats.TotalAchievements}\n\n" +
                   $"🏋️‍♂️ Un giorno alla volta, verso i miei obiettivi!\n" +
                   $"#CraftMyFit #MuscleGain #Fitness #Strength";

        private string GenerateWorkoutStreakText(UserStats stats) => $"🔥 Streak di {stats.CurrentStreak} giorni su CraftMyFit!\n\n" +
                   $"💪 La mia dedizione:\n" +
                   $"• Streak corrente: {stats.CurrentStreak} giorni\n" +
                   $"• Record personale: {stats.LongestStreak} giorni\n" +
                   $"• Totale allenamenti: {stats.CompletedWorkouts}\n\n" +
                   $"🎯 La costanza è la chiave del successo!\n" +
                   $"#CraftMyFit #WorkoutStreak #Consistency #Fitness";

        private async Task<string> GeneratePhotoComparisonTextAsync(int userId)
        {
            List<Models.Progress.ProgressPhoto> photos = await _progressPhotoRepository.GetByUserIdAsync(userId);
            int photoCount = photos.Count;

            if(photoCount < 2)
            {
                return string.Empty;
            }

            Models.Progress.ProgressPhoto? firstPhoto = photos.LastOrDefault();
            Models.Progress.ProgressPhoto? latestPhoto = photos.FirstOrDefault();

            if(firstPhoto == null || latestPhoto == null)
            {
                return string.Empty;
            }

            TimeSpan timeSpan = latestPhoto.Date - firstPhoto.Date;
            int daysApart = (int)timeSpan.TotalDays;

            return $"📸 La mia trasformazione in {daysApart} giorni!\n\n" +
                   $"✨ Risultati visibili:\n" +
                   $"• Prima foto: {firstPhoto.Date:dd/MM/yyyy}\n" +
                   $"• Ultima foto: {latestPhoto.Date:dd/MM/yyyy}\n" +
                   $"• Foto totali: {photoCount}\n\n" +
                   $"🌟 Il cambiamento è possibile!\n" +
                   $"#CraftMyFit #Transformation #Progress #Fitness";
        }

        private string GenerateMonthlyProgressText(UserStats stats) => $"📅 I miei progressi di questo mese con CraftMyFit!\n\n" +
                   $"🎯 Risultati mensili:\n" +
                   $"• Achievement: {stats.UnlockedAchievements} sbloccati\n" +
                   $"• Punti totali: {stats.TotalPoints}\n" +
                   $"• Streak corrente: {stats.CurrentStreak} giorni\n\n" +
                   $"📈 Ogni giorno è un passo avanti!\n" +
                   $"#CraftMyFit #MonthlyProgress #Fitness #Goals";

        private string GenerateYearlyProgressText(UserStats stats) => $"🎊 I miei risultati dell'anno con CraftMyFit!\n\n" +
                   $"🏆 Bilancio annuale:\n" +
                   $"• Allenamenti totali: {stats.CompletedWorkouts}\n" +
                   $"• Achievement: {stats.UnlockedAchievements}/{stats.TotalAchievements}\n" +
                   $"• Streak record: {stats.LongestStreak} giorni\n" +
                   $"• Progresso peso: {stats.WeightChangeText}\n\n" +
                   $"🌟 Un anno di crescita e dedizione!\n" +
                   $"#CraftMyFit #YearlyProgress #Fitness #Journey";

        private string GenerateGeneralProgressText(UserStats stats) => $"💪 I miei progressi fitness con CraftMyFit!\n\n" +
                   $"📊 Le mie statistiche:\n" +
                   $"• Allenamenti completati: {stats.CompletedWorkouts}\n" +
                   $"• Achievement sbloccati: {stats.UnlockedAchievements}\n" +
                   $"• Streak corrente: {stats.CurrentStreak} giorni\n" +
                   $"• Punti totali: {stats.TotalPoints}\n\n" +
                   $"🎯 Il viaggio continua!\n" +
                   $"#CraftMyFit #Fitness #Progress #Motivation";

        private string GenerateWorkoutPlanShareText(Models.Workout.WorkoutPlan workoutPlan)
        {
            string shareText = $"🏋️‍♂️ Condivido il mio piano di allenamento!\n\n" +
                          $"📋 {workoutPlan.Title}\n";

            if(!string.IsNullOrEmpty(workoutPlan.Description))
            {
                shareText += $"📝 {workoutPlan.Description}\n";
            }

            shareText += $"📅 Giorni: {workoutPlan.WorkoutDays?.Count ?? 0}\n" +
                        $"💪 Creato il: {workoutPlan.CreatedDate:dd/MM/yyyy}\n\n" +
                        $"🎯 Raggiungi i tuoi obiettivi con CraftMyFit!\n" +
                        $"#CraftMyFit #WorkoutPlan #Fitness #Training";

            return shareText;
        }

        private async Task<string?> CreateStatsImageAsync(int userId)
        {
            try
            {
                // In un'implementazione reale, qui creeresti un'immagine
                // con le statistiche dell'utente usando librerie come SkiaSharp

                // Per ora restituiamo null, indicando che la funzionalità
                // non è ancora implementata
                await Task.Delay(100);
                return null;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione immagine stats: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}