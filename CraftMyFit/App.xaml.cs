// App.xaml.cs - Classe App principale aggiornata
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
            if(serviceProvider != null)
            {
                _authService = serviceProvider.GetRequiredService<AuthService>();
                _preferenceService = serviceProvider.GetRequiredService<IPreferenceService>();
            }

            // Inizializza l'app
            InitializeApp();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
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

                // Verifica lo stato di autenticazione
                await CheckAuthenticationStatus();

                // Registra l'avvio dell'app
                RegisterAppLaunch();

                System.Diagnostics.Debug.WriteLine("App avviata con successo");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante l'avvio dell'app: {ex.Message}");
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            try
            {
                // Salva lo stato dell'app quando va in background
                SaveAppState();

                System.Diagnostics.Debug.WriteLine("App in background");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante il passaggio in background: {ex.Message}");
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();

            try
            {
                // Ripristina lo stato dell'app quando torna in foreground
                await RestoreAppState();

                // Verifica se il token è ancora valido
                if(_authService != null)
                {
                    _ = await _authService.IsTokenValidAsync();
                }

                System.Diagnostics.Debug.WriteLine("App ripresa dal background");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore durante il ripristino dal background: {ex.Message}");
            }
        }

        private void InitializeApp()
        {
            try
            {
                // Configura il tema dell'app
                SetAppTheme();

                // Configura le impostazioni di sistema
                ConfigureSystemSettings();

                // Inizializza i servizi critici
                InitializeCriticalServices();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione app: {ex.Message}");
            }
        }

        private void SetAppTheme()
        {
            try
            {
                if(_preferenceService != null)
                {
                    string savedTheme = _preferenceService.GetString("app_theme", "System");

                    UserAppTheme = savedTheme switch
                    {
                        "Light" => AppTheme.Light,
                        "Dark" => AppTheme.Dark,
                        _ => AppTheme.Unspecified
                    };
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'impostazione tema: {ex.Message}");
            }
        }

        private void ConfigureSystemSettings()
        {
            try
            {
                // Configura le impostazioni di sistema specifiche per piattaforma
#if ANDROID
                // Configurazioni specifiche per Android
                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
                {
                    if (handler.PlatformView is Android.Widget.EditText editText)
                    {
                        editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    }
                });
#endif

#if IOS
                // Configurazioni specifiche per iOS
                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
                {
                    if (handler.PlatformView is UIKit.UITextField textField)
                    {
                        textField.BorderStyle = UIKit.UITextBorderStyle.None;
                    }
                });
#endif
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella configurazione sistema: {ex.Message}");
            }
        }

        private void InitializeCriticalServices()
        {
            try
            {
                // Inizializza servizi che devono essere attivi da subito
                // (es. notifiche, sincronizzazione, ecc.)

                // Registra per le notifiche se autorizzato
                _ = Task.Run(async () =>
                {
                    IServiceProvider? serviceProvider = Handler?.MauiContext?.Services;
                    if(serviceProvider != null)
                    {
                        INotificationService? notificationService = serviceProvider.GetService<INotificationService>();
                        if(notificationService != null)
                        {
                            _ = await notificationService.RequestPermissionAsync();
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione servizi critici: {ex.Message}");
            }
        }

        private async Task CheckAuthenticationStatus()
        {
            try
            {
                if(_authService != null)
                {
                    bool isValidToken = await _authService.IsTokenValidAsync();

                    if(!isValidToken && _authService.IsAuthenticated)
                    {
                        // Token scaduto, effettua logout
                        _ = await _authService.LogoutAsync();

                        // Naviga alla schermata di login se necessario
                        await Shell.Current.GoToAsync("//login");
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo autenticazione: {ex.Message}");
            }
        }

        private void RegisterAppLaunch()
        {
            try
            {
                if(_preferenceService != null)
                {
                    // Incrementa il contatore di avvii
                    int launchCount = _preferenceService.GetInt("app_launch_count", 0);
                    _preferenceService.SetInt("app_launch_count", launchCount + 1);

                    // Registra la data dell'ultimo avvio
                    _preferenceService.SetDateTime("last_launch_date", DateTime.Now);

                    // Se è il primo avvio, registra la data di installazione
                    if(launchCount == 0)
                    {
                        _preferenceService.SetDateTime("first_launch_date", DateTime.Now);
                        _preferenceService.SetBool("first_time_user", true);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella registrazione avvio: {ex.Message}");
            }
        }

        private void SaveAppState()
        {
            try
            {
                if(_preferenceService != null)
                {
                    // Salva lo stato corrente dell'app
                    _preferenceService.SetDateTime("last_app_pause", DateTime.Now);

                    // Salva altre informazioni di stato se necessarie
                    string? currentPage = Shell.Current?.CurrentPage?.GetType().Name;
                    if(!string.IsNullOrEmpty(currentPage))
                    {
                        _preferenceService.SetString("last_visited_page", currentPage);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio stato: {ex.Message}");
            }
        }

        private async Task RestoreAppState()
        {
            try
            {
                if(_preferenceService != null)
                {
                    DateTime lastPause = _preferenceService.GetDateTime("last_app_pause", DateTime.MinValue);

                    // Se l'app è stata in pausa per più di 30 minuti, potrebbe essere necessario
                    // un refresh dei dati o una nuova autenticazione
                    if(lastPause != DateTime.MinValue && DateTime.Now - lastPause > TimeSpan.FromMinutes(30))
                    {
                        // Esegui operazioni di refresh se necessario
                        await RefreshAppData();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel ripristino stato: {ex.Message}");
            }
        }

        private async Task RefreshAppData()
        {
            try
            {
                // Refresh dei dati critici quando l'app torna dal background
                // dopo un periodo prolungato

                IServiceProvider? serviceProvider = Handler?.MauiContext?.Services;
                if(serviceProvider != null)
                {
                    // Esempio: refresh notifiche
                    INotificationService? notificationService = serviceProvider.GetService<INotificationService>();
                    if(notificationService != null)
                    {
                        _ = await notificationService.GetPendingNotificationsAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel refresh dati: {ex.Message}");
            }
        }

        public static void ChangeTheme(AppTheme theme)
        {
            if(Current is App app)
            {
                app.UserAppTheme = theme;

                // Salva la preferenza
                try
                {
                    IServiceProvider? serviceProvider = IPlatformApplication.Current?.Services;
                    IPreferenceService? preferenceService = serviceProvider?.GetService<IPreferenceService>();

                    if(preferenceService != null)
                    {
                        string themeString = theme switch
                        {
                            AppTheme.Light => "Light",
                            AppTheme.Dark => "Dark",
                            _ => "System"
                        };

                        preferenceService.SetString("app_theme", themeString);
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio tema: {ex.Message}");
                }
            }
        }
    }
}
