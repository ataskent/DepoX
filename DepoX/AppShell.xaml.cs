namespace DepoX;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Routing tanımları
        Routing.RegisterRoute("MenuPopup", typeof(Views.MenuPopup));
        Routing.RegisterRoute("TransferPage", typeof(Views.TransferPage));
        Routing.RegisterRoute("CountPage", typeof(DepoX.Features.Count.CountPage));
        Routing.RegisterRoute("ShipmentPage", typeof(Views.ShipmentPage));
        Routing.RegisterRoute("OptionsPage", typeof(Views.OptionsPage));
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MenuPopup");
    }

    private async void OnOptionsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("OptionsPage");
    }
}