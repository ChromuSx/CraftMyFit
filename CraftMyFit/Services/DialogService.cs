using CraftMyFit.Services.Interfaces;

namespace CraftMyFit.Services
{
    public class DialogService : IDialogService
    {
        [Obsolete]
        private Page? CurrentPage => Application.Current?.MainPage;

        public async Task<bool> ShowConfirmAsync(string title, string message, string accept = "OK", string cancel = "Annulla") => CurrentPage != null && await CurrentPage.DisplayAlert(title, message, accept, cancel);

        public async Task ShowAlertAsync(string title, string message, string accept = "OK")
        {
            if(CurrentPage == null)
            {
                return;
            }

            await CurrentPage.DisplayAlert(title, message, accept);
        }

        public async Task<string?> ShowPromptAsync(string title, string message, string placeholder = "", string accept = "OK", string cancel = "Annulla") => CurrentPage == null ? null : await CurrentPage.DisplayPromptAsync(title, message, accept, cancel, placeholder);

        public async Task<string?> ShowActionSheetAsync(string title, string? cancel, string? destruction, params string[] buttons) => CurrentPage == null ? null : await CurrentPage.DisplayActionSheet(title, cancel, destruction, buttons);
    }
}