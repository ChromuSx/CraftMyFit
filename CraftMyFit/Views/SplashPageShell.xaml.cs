namespace CraftMyFit.Views;

public partial class SplashPageShell : Shell
{
    public SplashPageShell()
    {
        InitializeComponent();

        // Registra solo la splash page
        Routing.RegisterRoute("splash", typeof(SplashPage));

        // Naviga immediatamente alla splash
        _ = GoToAsync("//splash");
    }
}