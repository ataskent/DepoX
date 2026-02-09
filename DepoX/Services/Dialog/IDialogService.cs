namespace DepoX.Services.Dialog;
public interface IDialogService
{
    Task ShowAlertAsync(
        string title,
        string message,
        string cancel = "Tamam");

    Task<bool> ShowConfirmAsync(
        string title,
        string message,
        string accept = "Evet",
        string cancel = "Hayır");
}

