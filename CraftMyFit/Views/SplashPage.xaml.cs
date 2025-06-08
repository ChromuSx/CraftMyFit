using CraftMyFit.Services.Authentication;

namespace CraftMyFit.Views
{
    public partial class SplashPage : ContentPage
    {
        private readonly AuthService _authService;

        // Costruttore senza parametri per DataTemplate
        public SplashPage()
        {
            InitializeComponent();

            // Ottieni il servizio dal service provider dell'app
            _authService = GetService<AuthService>();

            // Nascondi la tab bar per la splash page
            Shell.SetTabBarIsVisible(this, false);
        }

        // Costruttore con DI per quando viene creata tramite DI
        public SplashPage(AuthService authService) : this()
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        private T GetService<T>() where T : class
        {
            // Ottieni il service provider dall'applicazione corrente
            IServiceProvider serviceProvider = IPlatformApplication.Current?.Services
                ?? throw new InvalidOperationException("Service provider non disponibile");

            return serviceProvider.GetRequiredService<T>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeAppAsync();
        }

        private async Task InitializeAppAsync()
        {
            try
            {
                await UpdateStatus("Inizializzazione database...");
                await Task.Delay(800);

                await UpdateStatus("Caricamento configurazioni...");
                await Task.Delay(500);

                await UpdateStatus("Verifica connessione...");
                await Task.Delay(700);

                await UpdateStatus("Controllo autenticazione...");
                await Task.Delay(500);

                bool isAuthenticated = _authService.IsAuthenticated;
                bool isTokenValid = true;

                if (isAuthenticated)
                {
                    isTokenValid = await _authService.IsTokenValidAsync();
                }

                if (isAuthenticated && isTokenValid)
                {
                    await UpdateStatus("Accesso confermato...");
                    await Task.Delay(500);
                    // Naviga alla main app
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    if (isAuthenticated && !isTokenValid)
                    {
                        _ = await _authService.LogoutAsync();
                    }

                    await UpdateStatus("Redirect al login...");
                    await Task.Delay(500);
                    // Naviga al login
                    await Shell.Current.GoToAsync("//login");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nell'inizializzazione: {ex.Message}");
                await UpdateStatus("Errore di inizializzazione...");
                await Task.Delay(1000);
                await Shell.Current.GoToAsync("//login");
            }
        }

        private async Task UpdateStatus(string status)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (StatusLabel != null)
                {
                    StatusLabel.Text = status;
                }
            });
        }
    }
}