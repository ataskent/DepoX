namespace DepoX.Features.Basket;

public partial class BasketPage : ContentPage
{
    private readonly BasketViewModel _vm;
    private readonly IBasketService _basketService;
    private CancellationTokenSource? _saveCts;

    public BasketPage(
        BasketViewModel vm,
        IBasketService basketService)
    {
        InitializeComponent();

        _vm = vm;
        _basketService = basketService;
        BindingContext = _vm;
    }

    // ===============================
    // BARKOD OKUTMA
    // ===============================

    private void OnBarcodeCompleted(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text?.Trim();
        BarcodeEntry.Text = string.Empty;

        if (!string.IsNullOrEmpty(barcode))
            _vm.AddBarcode(barcode);

        BarcodeEntry.Focus();
    }

    // ===============================
    // BASKET SEÇÝMÝ
    // ===============================

    private void OnBasketSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is BasketVm basket)
        {
            _vm.SelectBasket(basket);
        }
    }

    private void OnCloseBasketPicker(object sender, EventArgs e)
    {
        _vm.CloseBasketPicker();
    }


    // ===============================
    // WHOUSE SEÇÝMÝ
    // ===============================

    private void OnWhouseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is WhouseVm whouse)
        {
            _vm.SelectWhouse(whouse);
        }
    }

    private void OnCloseWhousePicker(object sender, EventArgs e)
    {
        _vm.CloseWhousePicker();
    }

    // ===============================
    // BARKOD SÝLME
    // ===============================

    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is BasketItemVm item)
        {
            _vm.RemoveItem(item);
        }
    }

    // ===============================
    // KAYDET
    // ===============================

    private void OnSaveClicked(object sender, EventArgs e)
    {
        _ = SafeSaveAsync();
    }

    private async Task SafeSaveAsync()
    {
        if (_vm.IsBusy)
            return;

        if (!_vm.HasActiveBasket)
        {
            await DisplayAlert("Uyarý", "Sepet oluþturulmamýþ.", "Tamam");
            return;
        }

        if (_vm.SelectedWhouse == null)
        {
            await DisplayAlert("Uyarý", "Depo seçilmemiþ.", "Tamam");
            return;
        }

        if (_vm.Items.Count == 0)
        {
            await DisplayAlert("Uyarý", "Kaydedilecek satýr yok.", "Tamam");
            return;
        }

        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();

        try
        {
            _vm.IsBusy = true;

            var draft = _vm.ToModel();
            var result = await _basketService.SaveAsync(draft, _saveCts.Token);

            if (result.Success)
            {
                await DisplayAlert(
                    "Baþarýlý",
                    result.Message ?? "Sepet baþarýyla kaydedildi.",
                    "Tamam");

                _vm.ClearBasket();
            }
            else
            {
                await DisplayAlert(
                    "Hata",
                    result.Message ?? "ERP iþlemi baþarýsýz.",
                    "Tamam");
            }
        }
        catch (OperationCanceledException)
        {
            // Ýptal edildi, sessiz geç
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
        finally
        {
            _vm.IsBusy = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _vm.LoadInitialAsync();
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _saveCts?.Cancel();
    }
}
