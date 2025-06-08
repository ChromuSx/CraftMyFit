using CraftMyFit.Helpers.Utils;
using CraftMyFit.Services.Authentication;
using CraftMyFit.Services.Interfaces;

namespace CraftMyFit
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        private readonly IPreferenceService _preferenceService;

        public App()
        {
            InitializeComponent();

            // Ottieni i servizi dal DI container
            IServiceProvider? serviceProvider = IPlatformApplication.Current?.Services;
            if (serviceProvider != null)
            {
                _authService = serviceProvider.GetRequiredService<AuthService>();
                _preferenceService = serviceProvider.GetRequiredService<IPreferenceService>();
            }

            // Inizializza l'app
            InitializeApp();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Usa direttamente AppShell e inizia con la route splash
            Window window = new(new AppShell())
            {
                Title = "CraftMyFit"
            };

            // Configura le dimensioni della finestra per desktop
            const int newWidth = 400;
            const int newHeight = 800;

            window.X = ((DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density) - newWidth) / 2;
            window.Y = ((DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density) - newHeight) / 2;

            window.Width = newWidth;
            window.Height = newHeight;
            window.MinimumWidth = 350;
            window.MinimumHeight = 600;

            return window;
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                // Inizializza il database
                await DatabaseSetup.InitializeDatabaseAsync(Handler?.MauiContext?.Services);

                // Registra l'avvio dell'app
                RegisterAppLaunch();

                // Naviga alla splash page
                await Shell.Current.GoToAsync("//splash");

                System.Diagnostics.Debug.WriteLine("App avviata con successo");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante l'avvio dell'app: {ex.Message}");
            }
        }

        // Metodi deprecati rimossi - ora usiamo solo la navigazione Shell

        protected override void OnSleep()
        {
            base.OnSleep();
            try
            {
                SaveAppState();
                System.Diagnostics.Debug.WriteLine("App in background");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante il passaggio in background: {ex.Message}");
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();
            try
            {
                RestoreAppState();

                // Verifica che l'utente sia ancora autenticato al ritorno
                if (_authService != null && _authService.IsAuthenticated)
                {
                    bool isTokenValid = await _authService.IsTokenValidAsync();
                    if (!isTokenValid)
                    {
                        // Token scaduto, torna al login
                        _ = await _authService.LogoutAsync();
                        await Shell.Current.GoToAsync("//login");
                    }
                }

                System.Diagnostics.Debug.WriteLine("App ripresa dal background");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante il ripristino dal background: {ex.Message}");
            }
        }

        private void InitializeApp()
        {
            try
            {
                SetAppTheme();
                ConfigureSystemSettings();
                InitializeCriticalServices();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione app: {ex.Message}");
            }
        }

        private void RegisterAppLaunch() { /* implementazione esistente */ }
        private void SaveAppState() { /* implementazione esistente */ }
        private void RestoreAppState() { /* implementazione esistente */ }
        private void SetAppTheme() { /* implementazione esistente */ }
        private void ConfigureSystemSettings() { /* implementazione esistente */ }
        private void InitializeCriticalServices() { /* implementazione esistente */ }
    }
}