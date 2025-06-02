using CraftMyFit.Services.Interfaces;

namespace CraftMyFit.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route) => await Shell.Current.GoToAsync(route);

        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters) => await Shell.Current.GoToAsync(route, parameters);

        public async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        public async Task GoToRootAsync() => await Shell.Current.GoToAsync("//");

        public Task<bool> CanGoBackAsync()
        {
            IReadOnlyList<Page>? navigationStack = Shell.Current?.Navigation?.NavigationStack;
            return Task.FromResult(navigationStack?.Count > 1);
        }
    }
}