using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using DepoX.Services.Count;
using DepoX.Services.Erp;
using DepoX.Views;
using Microsoft.Extensions.DependencyInjection;

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
            builder.Services.AddSingleton<Services.LocalDataService>(sp =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "local.db");
                return new Services.LocalDataService(dbPath);
            });

            // ===== Network =====
            builder.Services.AddHttpClient();

            // ===== ERP =====
            builder.Services.AddSingleton(new HttpClient());
            builder.Services.AddSingleton<IErpGateway, SoapErpGateway>();

            // ===== Application Services =====
            builder.Services.AddTransient<ICountService, CountService>();



            // ===== Pages =====
            builder.Services.AddTransient<CountPage>();

            // ===== Sync / Device =====
            builder.Services.AddSingleton<Services.SyncService>();
            builder.Services.AddSingleton<Services.BarcodeScannerService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
