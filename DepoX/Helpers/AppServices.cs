using System;

namespace DepoX.Helpers;

/// <summary>
/// Basit servis erişimi. Shell route'larında DI tam olarak devreye girmeden
/// Pages/ViewModels içinden singleton servisleri çekebilmek için.
/// </summary>
public static class AppServices
{
    public static IServiceProvider Current
        => Application.Current?.Handler?.MauiContext?.Services
           ?? throw new InvalidOperationException("MAUI Services henüz hazır değil.");

    public static T Get<T>() where T : notnull =>
        Current.GetService(typeof(T)) is T t
            ? t
            : throw new InvalidOperationException($"Service bulunamadı: {typeof(T).FullName}");
}
