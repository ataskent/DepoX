namespace DepoX;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        // ===== OPERASYON MODÜLLERİ =====
        Routing.RegisterRoute(
            nameof(Features.Count.CountPage),
            typeof(Features.Count.CountPage));

        Routing.RegisterRoute(
            nameof(Features.Split.SplitPage),
            typeof(Features.Split.SplitPage));

        Routing.RegisterRoute(
            nameof(Features.Basket.BasketPage),
            typeof(Features.Basket.BasketPage));
    }
}
