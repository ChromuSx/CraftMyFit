using CraftMyFit.Services.Interfaces;
using CraftMyFit.Views.Exercises;
using CraftMyFit.Views.Progress;
using CraftMyFit.Views.Workout;

namespace CraftMyFit
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            ConfigureShell();
        }

        private static void RegisterRoutes()
        {
            // Registra le rotte per le pagine principali (non tab)

            // Workout Pages
            Routing.RegisterRoute("createworkoutplan", typeof(CreateWorkoutPlanPage));
            Routing.RegisterRoute("editworkoutplan", typeof(EditWorkoutPlanPage));
            Routing.RegisterRoute("workoutplandetail", typeof(WorkoutPlanDetailPage));
            Routing.RegisterRoute("workoutexecution", typeof(WorkoutExecutionPage));

            // Exercise Pages
            Routing.RegisterRoute("exercisedetail", typeof(ExerciseDetailPage));
            Routing.RegisterRoute("exerciseform", typeof(ExerciseFormPage));

            // Progress Pages
            Routing.RegisterRoute("addprogressphoto", typeof(AddProgressPhotoPage));
            Routing.RegisterRoute("bodymeasurements", typeof(BodyMeasurementsPage));

            // Auth Pages (se implementate)
            // Routing.RegisterRoute("login", typeof(LoginPage));
            // Routing.RegisterRoute("register", typeof(RegisterPage));
            // Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));

            // Modal/Popup Pages
            // Routing.RegisterRoute("exercisetimer", typeof(ExerciseTimerPage));
            // Routing.RegisterRoute("photogallery", typeof(PhotoGalleryPage));
            // Routing.RegisterRoute("settings/profile", typeof(ProfilePage));
            // Routing.RegisterRoute("settings/notifications", typeof(NotificationSettingsPage));

            System.Diagnostics.Debug.WriteLine("Routes registrate con successo");
        }

        private void ConfigureShell()
        {
            try
            {
                // Configura l'aspetto della Shell
                ConfigureShellAppearance();

                // Configura la navigazione
                ConfigureNavigation();

                // Configura i handler per gli eventi di navigazione
                ConfigureNavigationHandlers();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella configurazione Shell: {ex.Message}");
            }
        }

        private void ConfigureShellAppearance()
        {
            try
            {
                // Configura i colori e l'aspetto della TabBar
                Shell.SetTabBarBackgroundColor(this, Colors.White);
                Shell.SetTabBarForegroundColor(this, Color.FromArgb("#512BD4"));
                Shell.SetTabBarTitleColor(this, Colors.Black);
                Shell.SetTabBarUnselectedColor(this, Colors.Gray);

                // Configura la NavBar
                Shell.SetNavBarHasShadow(this, true);
                Shell.SetNavBarIsVisible(this, true);

                // Configura il FlyoutBehavior se necessario
                FlyoutBehavior = FlyoutBehavior.Disabled;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella configurazione aspetto Shell: {ex.Message}");
            }
        }

        private void ConfigureNavigation()
        {
            try
            {
                // Configura la navigazione predefinita
                // Imposta la tab di default
                CurrentItem = Items.FirstOrDefault();

                // Configura eventuali route predefinite
                // GoToAsync("//dashboard");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella configurazione navigazione: {ex.Message}");
            }
        }

        private void ConfigureNavigationHandlers()
        {
            try
            {
                // Handler per il cambio di navigazione
                Navigating += OnShellNavigating;
                Navigated += OnShellNavigated;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella configurazione handler navigazione: {ex.Message}");
            }
        }

        private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Navigazione verso: {e.Target.Location}");

                // Qui puoi aggiungere logica per controllare la navigazione
                // Ad esempio, verificare se l'utente può lasciare la pagina corrente

                // Esempio: previeni navigazione se ci sono modifiche non salvate
                // if (HasUnsavedChanges())
                // {
                //     e.Cancel();
                //     return;
                // }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante la navigazione: {ex.Message}");
            }
        }

        private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Navigazione completata: {e.Current.Location}");

                // Qui puoi aggiungere logica post-navigazione
                // Ad esempio, analytics, cleanup, refresh dati

                // Registra la navigazione per analytics
                RegisterPageVisit(e.Current.Location.ToString());
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore post-navigazione: {ex.Message}");
            }
        }

        private void RegisterPageVisit(string pageName)
        {
            try
            {
                // Registra la visita della pagina per analytics
                IServiceProvider? serviceProvider = Handler?.MauiContext?.Services;
                IPreferenceService? preferenceService = serviceProvider?.GetService<IPreferenceService>();

                if(preferenceService != null)
                {
                    int visitCount = preferenceService.GetInt($"page_visit_{pageName}", 0);
                    preferenceService.SetInt($"page_visit_{pageName}", visitCount + 1);
                    preferenceService.SetDateTime($"last_visit_{pageName}", DateTime.Now);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella registrazione visita pagina: {ex.Message}");
            }
        }

        // Metodi helper per la navigazione programmatica
        public static async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
        {
            try
            {
                if(parameters != null)
                {
                    await Current.GoToAsync(route, parameters);
                }
                else
                {
                    await Current.GoToAsync(route);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella navigazione verso {route}: {ex.Message}");
            }
        }

        public static async Task GoBackAsync()
        {
            try
            {
                await Current.GoToAsync("..");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel tornare indietro: {ex.Message}");
            }
        }

        public static async Task GoToRootAsync()
        {
            try
            {
                await Current.GoToAsync("//");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel tornare alla root: {ex.Message}");
            }
        }

        // Metodi per la gestione dello stato della Shell
        public void ShowTabBar() => Shell.SetTabBarIsVisible(this, true);

        public void HideTabBar() => Shell.SetTabBarIsVisible(this, false);

        public void EnableFlyout() => FlyoutBehavior = FlyoutBehavior.Flyout;

        public void DisableFlyout() => FlyoutBehavior = FlyoutBehavior.Disabled;

        // Gestione del ciclo di vita
        protected override void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("AppShell appeared");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            System.Diagnostics.Debug.WriteLine("AppShell disappeared");
        }
    }
}