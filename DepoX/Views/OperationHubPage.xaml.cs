namespace DepoX.Views;

public partial class OperationHubPage : ContentPage
{
    public OperationHubPage()
    {
        InitializeComponent();
    }

    private async void OnCountClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(Features.Count.CountPage));
    }

    private async void OnSplitClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(Features.Split.SplitPage));
    }

    private async void OnBasketClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(Features.Basket.BasketPage));
    }
}
