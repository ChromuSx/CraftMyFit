// MauiProgram.cs - Aggiornato con registrazione pagine di autenticazione
using CraftMyFit.Data;
using CraftMyFit.Data.Interfaces;
using CraftMyFit.Data.Repositories;
using CraftMyFit.Services;
using CraftMyFit.Services.Authentication;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Dashboard;
using CraftMyFit.ViewModels.Exercises;
using CraftMyFit.ViewModels.Progress;
using CraftMyFit.ViewModels.Settings;
using CraftMyFit.ViewModels.Workout;
using CraftMyFit.Views; // Per le pagine di autenticazione
using CraftMyFit.Views.Dashboard;
using CraftMyFit.Views.Exercises;
using CraftMyFit.Views.Progress;
using CraftMyFit.Views.Settings;
using CraftMyFit.Views.Workout;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CraftMyFit
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configura il database
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "craftmyfit.db");
            builder.Services.AddDbContext<CraftMyFitDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Registra servizi principali
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IPreferenceService, PreferenceService>();
            builder.Services.AddSingleton<IPhotoService, PhotoService>();
            builder.Services.AddSingleton<IFileService, FileService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            // Registra servizi di autenticazione
            builder.Services.AddSingleton<AuthService>();

            // Registra servizi wearable
            builder.Services.AddSingleton<IWearableDeviceService, WearableDeviceService>();

            // Registra repository
            builder.Services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>();
            builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
            builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
            builder.Services.AddScoped<IBodyMeasurementRepository, BodyMeasurementRepository>();
            builder.Services.AddScoped<IProgressPhotoRepository, ProgressPhotoRepository>();

            // Registra ViewModels principali
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ExercisesViewModel>();
            builder.Services.AddTransient<ExerciseDetailViewModel>();
            builder.Services.AddTransient<ExerciseFormViewModel>();
            builder.Services.AddTransient<WorkoutPlansViewModel>();
            builder.Services.AddTransient<WorkoutPlanDetailViewModel>();
            builder.Services.AddTransient<CreateWorkoutPlanViewModel>();
            builder.Services.AddTransient<EditWorkoutPlanViewModel>();
            builder.Services.AddTransient<WorkoutExecutionViewModel>();
            builder.Services.AddTransient<ProgressPhotosViewModel>();
            builder.Services.AddTransient<AddProgressPhotoViewModel>();
            builder.Services.AddTransient<BodyMeasurementsViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // Registra Pages principali (esistenti del progetto)
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ExercisesPage>();
            builder.Services.AddTransient<ExerciseDetailPage>();
            builder.Services.AddTransient<ExerciseFormPage>();
            builder.Services.AddTransient<WorkoutPlansPage>();
            builder.Services.AddTransient<WorkoutPlanDetailPage>();
            builder.Services.AddTransient<CreateWorkoutPlanPage>();
            builder.Services.AddTransient<EditWorkoutPlanPage>();
            builder.Services.AddTransient<WorkoutExecutionPage>();
            builder.Services.AddTransient<ProgressPhotosPage>();
            builder.Services.AddTransient<AddProgressPhotoPage>();
            builder.Services.AddTransient<BodyMeasurementsPage>();
            builder.Services.AddTransient<Views.Settings.SettingsPage>();

            // === REGISTRA PAGINE DI AUTENTICAZIONE (NUOVE) ===
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SplashPage>();

            // Pagine opzionali (se create)
            try
            {
                builder.Services.AddTransient<WorkoutDetailPage>();
                builder.Services.AddTransient<EditProfilePage>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pagine opzionali non registrate: {ex.Message}");
            }

            // Registra servizi HTTP per API (se necessari)
            builder.Services.AddHttpClient("CraftMyFitApi", client =>
            {
                client.BaseAddress = new Uri("https://api.craftmyfit.com/v1/");
                client.DefaultRequestHeaders.Add("User-Agent", "CraftMyFit-Mobile/1.0");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            MauiApp app = builder.Build();

            // Inizializza il database all'avvio
            Task.Run(async () =>
            {
                try
                {
                    using IServiceScope scope = app.Services.CreateScope();
                    CraftMyFitDbContext dbContext = scope.ServiceProvider.GetRequiredService<CraftMyFitDbContext>();
                    await dbContext.Database.EnsureCreatedAsync();

                    System.Diagnostics.Debug.WriteLine("Database inizializzato con successo");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione del database: {ex.Message}");
                }
            });

            return app;
        }
    }
}