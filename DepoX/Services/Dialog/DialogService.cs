using Microsoft.Maui.Controls;

namespace DepoX.Services.Dialog;

public class DialogService : IDialogService
{
    private Page? CurrentPage =>
        Application.Current?
            .Windows.FirstOrDefault()?
            .Page;

    public async Task ShowAlertAsync(
        string title,
        string message,
        string cancel = "Tamam")
    {
        var page = CurrentPage;
        if (page == null) return;

        await MainThread.InvokeOnMainThreadAsync(() =>
            page.DisplayAlert(title, message, cancel));
    }

    public async Task<bool> ShowConfirmAsync(
        string title,
        string message,
        string accept = "Evet",
        string cancel = "Hayır")
    {
        var page = CurrentPage;
        if (page == null) return false;

        return await MainThread.InvokeOnMainThreadAsync(() =>
            page.DisplayAlert(title, message, accept, cancel));
    }
}
