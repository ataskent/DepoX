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

//        await DisplayAlert("Sayım Kaydedildi", summary, "Tamam");
//    }
//}

using DepoX.Dtos;
using DepoX.Models;
using DepoX.Services.Count;
using DepoX.ViewModels;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DepoX.Views;

public partial class CountPage : ContentPage
{
    public CountViewModel VM => BindingContext as CountViewModel;

    private readonly ICountService _countService;

    public CountPage(ICountService countService)
    {
        InitializeComponent();
        _countService = countService;
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

        if (VM.CountItems.Count == 0)
        {
            await DisplayAlert("Uyarı", "En az 1 barkod okutmalısın.", "Tamam");
            return;
        }


        var draft = new CountDraftDto
        {
            ClientDraftId = Guid.NewGuid().ToString(),
            WorkplaceCode = "01",
            Items = VM.CountItems.Select(x => new CountDraftItemDto
            {
                Barcode = x.Barcode,
                Quantity = x.Quantity,
                StockCode = "",
                FromWarehouseCode = "",
                LocationCode = ""
            }).ToList()
        };

        try
        {
            await _countService.SaveDraftAsync(draft);
            await DisplayAlert("OK", "Barkodlar ERP'ye gönderildi.", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

}