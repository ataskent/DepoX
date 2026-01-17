namespace DepoX.Views;

public partial class OptionsPage : ContentPage
{
    public OptionsPage()
    {
        InitializeComponent();
    }

    private async void OnDarkModeClicked(object sender, EventArgs e)
    {
        Application.Current.UserAppTheme = AppTheme.Dark;
        await DisplayAlert("Tema", "Karanlýk mod aktif.", "Tamam");
    }

    private async void OnLightModeClicked(object sender, EventArgs e)
    {
        Application.Current.UserAppTheme = AppTheme.Light;
        await DisplayAlert("Tema", "Açýk mod aktif.", "Tamam");
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Hakkýnda", "DepoX v1.0 - .NET MAUI", "Kapat");
    }
}