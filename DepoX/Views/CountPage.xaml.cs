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

using DepoX.Models;
using DepoX.ViewModels;
using System.Text.Json;

using System.Net.Http;
using System.Text;

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

        if (VM.CountItems.Count == 0)
        {
            await DisplayAlert("Uyarı", "En az 1 barkod okutmalısın.", "Tamam");
            return;
        }

        // 🔴 SERVER'A GİDECEK EN BASİT SEPET
        var draft = new
        {
            ClientDraftId = Guid.NewGuid().ToString(),
            BasketId = "",                // boş → server yeni sepet açacak
            WorkplaceCode = "01",
            Items = VM.CountItems.Select(x => new
            {
                Barcode = x.Barcode,
                StockCode = "",            // şimdilik boş
                Quantity = (decimal)x.Quantity,
                FromWarehouseCode = "",    // şimdilik boş
                LocationCode = ""
            }).ToArray()
        };

        try
        {
            var json = JsonSerializer.Serialize(new { draft });

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            // ⚠️ DİKKAT: asmx DOSYA yolu
            var url = "http://10.41.1.174:8061/customprg/xml/terminalservice.asmx/SaveBasket";

            using var client = new HttpClient();
            var response = await client.PostAsync(url, content);
            var resultText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Servis Hata", resultText, "Tamam");
                return;
            }

            await DisplayAlert("OK", "Barkodlar Uyumsoft’a gönderildi.", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }
}