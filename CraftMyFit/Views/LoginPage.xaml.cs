using CraftMyFit.Services.Authentication;

namespace CraftMyFit.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        // Costruttore senza parametri per DataTemplate
        public LoginPage()
        {
            InitializeComponent();

            // Ottieni il servizio dal service provider dell'app
            _authService = GetService<AuthService>();
        }

        // Costruttore con DI per quando viene creata tramite DI
        public LoginPage(AuthService authService) : this()
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

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
                button.Text = "Accesso in corso...";

                try
                {
                    AuthResult result = await _authService.LoginAsync("demo@craftmyfit.com", "demo123");

                    if (!result.Success)
                    {
                        await DisplayAlert("Errore", result.ErrorMessage ?? "Errore di login", "OK");
                    }
                    // La navigazione viene gestita automaticamente dall'AuthService
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Errore", $"Errore durante il login: {ex.Message}", "OK");
                }
                finally
                {
                    button.IsEnabled = true;
                    button.Text = "Accedi";
                }
            }
        }
    }
}