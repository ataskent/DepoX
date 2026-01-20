using CommunityToolkit.Maui;
using DepoX.Features.Count;
using DepoX.Features.Split;
using DepoX.Services.Erp;
using DepoX.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DepoX
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ===== Local / Offline =====
          
            // ===== Network =====
            builder.Services.AddHttpClient();
            // ===== ERP =====
            builder.Services.AddSingleton(new HttpClient());
            builder.Services.AddSingleton<IErpGateway, ErpGateway>();
            //builder.Services.AddSingleton<IBarcodeCache, BarcodeCache>();
            // ===== Application Services =====
            builder.Services.AddTransient<ICountService, CountService>();
            builder.Services.AddTransient<ISplitService, SplitService>();
            // ===== Pages =====
            builder.Services.AddTransient<SplitPage>();
            builder.Services.AddTransient<SplitViewModel>();
            builder.Services.AddTransient<CountPage>();
            builder.Services.AddTransient<CountViewModel>();

            // ===== Sync / Device =====

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
