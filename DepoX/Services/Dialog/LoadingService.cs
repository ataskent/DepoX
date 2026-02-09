
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace DepoX.Services.Dialog;

public class LoadingService : ILoadingService
{
    private int _counter = 0;
    private ContentView? _overlay;

    private ContentPage? GetCurrentPage()
    {
        // 1️⃣ Shell varsa
        if (Shell.Current?.CurrentPage is ContentPage shellPage)
            return shellPage;

        // 2️⃣ Window üzerinden
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window?.Page is ContentPage windowPage)
            return windowPage;

        return null;
    }

    public void Show(string? message = null)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _counter++;
            if (_overlay != null) return;

            var page = GetCurrentPage();
            if (page == null) return;

            _overlay = CreateOverlay(message);

            // 🟢 Content her zaman Grid içine alınır
            if (page.Content is not Grid rootGrid)
            {
                rootGrid = new Grid();
                var original = page.Content;
                page.Content = null;

                rootGrid.Children.Add(original);
                page.Content = rootGrid;
            }

            rootGrid.Children.Add(_overlay);
        });
    }

    public void Hide()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _counter--;
            if (_counter > 0) return;

            _counter = 0;

            if (_overlay?.Parent is Layout layout)
                layout.Children.Remove(_overlay);

            _overlay = null;
        });
    }

    private ContentView CreateOverlay(string? message)
    {
        return new ContentView
        {
            BackgroundColor = Color.FromArgb("#80000000"),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Content = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 12,
                Children =
                {
                    new ActivityIndicator
                    {
                        IsRunning = true,
                        WidthRequest = 48,
                        HeightRequest = 48,
                        Color = Colors.White
                    },
                    new Label
                    {
                        Text = message ?? "İşlem yapılıyor...",
                        TextColor = Colors.White,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        IsVisible = !string.IsNullOrEmpty(message)
                    }
                }
            }
        };
    }
}
