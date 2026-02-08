using DepoX.Services.Erp;

namespace DepoX;

public partial class AppShell : Shell
{
    private readonly IErpGateway _erpGateway;
    private bool _hasCheckedServices;

    public AppShell(IErpGateway erpGateway)
    {
        InitializeComponent();
        _erpGateway = erpGateway;

        RegisterRoutes();
        Loaded += OnLoaded;
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute(
            nameof(Features.Count.CountPage),
            typeof(Features.Count.CountPage));

        Routing.RegisterRoute(
            nameof(Features.Split.SplitPage),
            typeof(Features.Split.SplitPage));

        Routing.RegisterRoute(
            nameof(Features.Basket.BasketPage),
            typeof(Features.Basket.BasketPage));

        Routing.RegisterRoute(
            nameof(Features.FromWorder.FromWorderPage),
            typeof(Features.FromWorder.FromWorderPage));
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (_hasCheckedServices)
            return;

        _hasCheckedServices = true;
        Loaded -= OnLoaded;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));

        var isConnected = await _erpGateway.IsConnectAsync(cts.Token);

        if (!isConnected)
        {
            await DisplayAlert(
                "Ağ Bağlantısı Yok",
                "ERP servisine ulaşılamıyor.\n\nLütfen Wi-Fi / VPN bağlantısını kontrol edin.",
                "Tamam");
        }
    }
}
