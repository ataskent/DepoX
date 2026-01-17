//using DepoX.Models;
//using DepoX.ViewModels;

//namespace DepoX.Views;

//public partial class CountPage : ContentPage
//{
//    public CountViewModel VM => BindingContext as CountViewModel;

//    public CountPage()
//    {
//        InitializeComponent();
//    }

//    private void OnBarcodeEntered(object sender, EventArgs e)
//    {
//        var barcode = BarcodeEntry.Text?.Trim();
//        if (string.IsNullOrEmpty(barcode)) return;

//        var existing = VM.CountItems.FirstOrDefault(x => x.Barcode == barcode);
//        if (existing != null)
//        {
//            existing.Quantity++;
//        }
//        else
//        {
//            VM.CountItems.Add(new CountItem
//            {
//                Barcode = barcode,
//                Quantity = 1,
//                Color = "",
//                Quality = ""
//            });
//        }

//        BarcodeEntry.Text = "";
//    }

//    private void OnAddNewClicked(object sender, EventArgs e)
//    {
//        VM.CountItems.Add(new CountItem
//        {
//            Barcode = "",
//            Quantity = 0,
//            Color = "",
//            Quality = ""
//        });
//        VM.IsEditing = true;
//    }

//    private void OnEditClicked(object sender, EventArgs e)
//    {
//        VM.IsEditing = !VM.IsEditing;
//    }

//    private void OnDeleteClicked(object sender, EventArgs e)
//    {
//        if (sender is Button btn && btn.CommandParameter is CountItem item)
//        {
//            VM.CountItems.Remove(item);
//        }
//    }

//    private async void OnSaveClicked(object sender, EventArgs e)
//    {
//        VM.IsEditing = false;

//        var summary = string.Join("\n", VM.CountItems.Select(x =>
//            $"Barkod: {x.Barcode}, Miktar: {x.Quantity}, Renk: {x.Color}, Kalite: {x.Quality}"));

//        await DisplayAlert("Sayým Kaydedildi", summary, "Tamam");
//    }
//}

using DepoX.Models;
using DepoX.ViewModels;

namespace DepoX.Views;

public partial class CountPage : ContentPage
{
    public CountViewModel VM => BindingContext as CountViewModel;

    public CountPage()
    {
        InitializeComponent();
    }

    private void OnBarcodeEntered(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text?.Trim();
        if (string.IsNullOrEmpty(barcode)) return;

        var existing = VM.CountItems.FirstOrDefault(x => x.Barcode == barcode);
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            VM.CountItems.Add(new CountItem
            {
                Barcode = barcode,
                Quantity = 1,
                Color = "",
                Quality = ""
            });
        }

        BarcodeEntry.Text = "";
    }

    private void OnAddItemInvoked(object sender, EventArgs e)
    {
        VM.CountItems.Add(new CountItem
        {
            Barcode = "",
            Quantity = 0,
            Color = "",
            Quality = ""
        });
        VM.IsEditing = true;
    }

    private void OnEditItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe && swipe.CommandParameter is CountItem item)
        {
            // Burada item üzerinde düzenleme açabilirsin
            VM.IsEditing = true;
        }
    }

    private void OnDeleteItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe && swipe.CommandParameter is CountItem item)
        {
            VM.CountItems.Remove(item);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        VM.IsEditing = false;

        var summary = string.Join("\n", VM.CountItems.Select(x =>
            $"Barkod: {x.Barcode}, Miktar: {x.Quantity}, Renk: {x.Color}, Kalite: {x.Quality}"));

        await DisplayAlert("Sayým Kaydedildi", summary, "Tamam");
    }
}