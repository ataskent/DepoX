namespace DepoX.Views;

public partial class MenuPopup : ContentPage
{
    public MenuPopup()
    {
        InitializeComponent();
    }

    private async void OnTransferClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TransferPage");
    }

    private async void OnCountClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("CountPage");
    }

    private async void OnShipmentClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ShipmentPage");
    }
}