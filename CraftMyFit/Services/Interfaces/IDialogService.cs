namespace CraftMyFit.Services.Interfaces
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmAsync(string title, string message, string accept = "OK", string cancel = "Annulla");
        Task ShowAlertAsync(string title, string message, string accept = "OK");
        Task<string?> ShowPromptAsync(string title, string message, string placeholder = "", string accept = "OK", string cancel = "Annulla");
        Task<string?> ShowActionSheetAsync(string title, string? cancel, string? destruction, params string[] buttons);
    }
}