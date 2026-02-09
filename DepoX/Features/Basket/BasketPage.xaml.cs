using DepoX.Services.Dialog;

namespace DepoX.Features.Basket;

public partial class BasketPage : ContentPage
{
    private readonly BasketViewModel _vm;
    private readonly IBasketService _basketService;
    private CancellationTokenSource? _saveCts; 
    private readonly ILoadingService _loading;
    private readonly IDialogService _dialog;


    public BasketPage(
        BasketViewModel vm,
        IBasketService basketService,
        ILoadingService loadingService,
        IDialogService dialogService)
    {
        InitializeComponent();

        _vm = vm;
        _basketService = basketService;
        _loading = loadingService;
        _dialog = dialogService;
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
            _vm.Items.Clear();
            _vm.ValidatedStocks.Clear();
            _vm.SelectBasket(basket);
        }
    }

    private void OnCloseBasketPicker(object sender, EventArgs e)
    {
        _vm.CloseBasketPicker();
    }


    // ===============================
    // DEPO SEÇÝMÝ
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

        // ===============================
        // ÖN KONTROLLER
        // ===============================

        if (!_vm.HasActiveBasket)
        {
            await _dialog.ShowAlertAsync(
                "Uyarý",
                "Sepet oluþturulmamýþ.");
            return;
        }

        if (_vm.SelectedWhouse == null)
        {
            await _dialog.ShowAlertAsync(
                "Uyarý",
                "Depo seçilmemiþ.");
            return;
        }

        if (_vm.Items.Count == 0)
        {
            await _dialog.ShowAlertAsync(
                "Uyarý",
                "Kaydedilecek satýr yok.");
            return;
        }

        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();

        try
        {
            _vm.IsBusy = true;

            // ===============================
            // LOADING AÇ
            // ===============================
            _loading.Show("Sepet kaydediliyor...");

            await Task.Yield();

            var draft = _vm.ToModel();
            var result = await _basketService
                .SaveAsync(draft, _saveCts.Token);

            // ===============================
            // ERP SONUCU
            // ===============================

            if (result == null)
            {
                await _dialog.ShowAlertAsync(
                    "Hata",
                    "ERP baðlantýsý kurulamadý.");
                return;
            }

            if (!result.Success)
            {
                await _dialog.ShowAlertAsync(
                    "Hata",
                    result.Message ?? "ERP iþlemi baþarýsýz.");
                return;
            }

            // ===============================
            // BAÞARILI
            // ===============================

            await _dialog.ShowAlertAsync(
                "Baþarýlý",
                result.Message ?? "Sepet baþarýyla kaydedildi.");

            // Ýstersen burada açarsýn
            // _vm.ClearBasket();
        }
        catch (OperationCanceledException)
        {
            // Bilinçli iptal ? sessiz geç
        }
        catch (Exception ex)
        {
            await _dialog.ShowAlertAsync(
                "Hata",
                ex.Message);
        }
        finally
        {
            // ===============================
            // LOADING KAPAT (GARANTÝ)
            // ===============================
            _loading.Hide();
            _vm.IsBusy = false;
        }
    }


    //private async Task SafeSaveAsync()
    //{
    //    if (_vm.IsBusy)
    //        return;

    //    if (!_vm.HasActiveBasket)
    //    {
    //        await DisplayAlert("Uyarý", "Sepet oluþturulmamýþ.", "Tamam");
    //        return;
    //    }

    //    if (_vm.SelectedWhouse == null)
    //    {
    //        await DisplayAlert("Uyarý", "Depo seçilmemiþ.", "Tamam");
    //        return;
    //    }

    //    if (_vm.Items.Count == 0)
    //    {
    //        await DisplayAlert("Uyarý", "Kaydedilecek satýr yok.", "Tamam");
    //        return;
    //    }

    //    _saveCts?.Cancel();
    //    _saveCts = new CancellationTokenSource();

    //    try
    //    {
    //        _vm.IsBusy = true;

    //        var draft = _vm.ToModel();
    //        var result = await _basketService.SaveAsync(draft, _saveCts.Token);

    //        if (result.Success)
    //        {
    //            await DisplayAlert(
    //                "Baþarýlý",
    //                result.Message ?? "Sepet baþarýyla kaydedildi.",
    //                "Tamam");

    //            //_vm.ClearBasket();
    //        }
    //        else
    //        {
    //            await DisplayAlert(
    //                "Hata",
    //                result.Message ?? "ERP iþlemi baþarýsýz.",
    //                "Tamam");
    //        }
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        // Ýptal edildi, sessiz geç
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Hata", ex.Message, "Tamam");
    //    }
    //    finally
    //    {
    //        _vm.IsBusy = false;
    //    }
    //}

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
