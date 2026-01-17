using CommunityToolkit.Maui;
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

			// ===== Services (Singleton) =====
			// Offline DB
			builder.Services.AddSingleton(sp =>
			{
				var dbPath = Path.Combine(FileSystem.AppDataDirectory, "local.db");
				return new Services.LocalDataService(dbPath);
			});

			// Network client (ileride SOAP'a dönecek)
			builder.Services.AddSingleton(new HttpClient());
			builder.Services.AddSingleton<Services.SyncService>();
			builder.Services.AddSingleton<Services.BarcodeScannerService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
