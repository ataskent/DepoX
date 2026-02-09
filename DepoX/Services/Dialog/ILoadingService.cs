namespace DepoX.Services.Dialog;
public interface ILoadingService
{
    void Show(string? message = null);
    void Hide();
}
