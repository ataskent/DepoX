using DepoX.Services.Dialog;

namespace DepoX.Features.Count;

public partial class CountPage : ContentPage
{
    private readonly CountViewModel _vm;
    private readonly ICountService _countService;
    private readonly ILoadingService _loadingService;
    private CancellationTokenSource? _saveCts;

    public CountPage(
        CountViewModel vm,
        ICountService countService, ILoadingService loadingService)
    {
        InitializeComponent();

        _vm = vm;
        _countService = countService;
        _loadingService = loadingService;
        BindingContext = _vm;
    }

    // ===============================
    // DEPO SEÇİMİ
    // ===============================

    private void OnCountSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CountVm count)
        {
            _vm.SelectCount(count);
        }
    }

    private void OnCloseCountPicker(object sender, EventArgs e)
    {
        _vm.CloseCountPicker();
    }

    // ===============================
    // DEPO SEÇİMİ
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

    // Barkod ENTER
    private void OnBarcodeCompleted(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text?.Trim();

        BarcodeEntry.Text = string.Empty;
        BarcodeEntry.Focus();

        if (!string.IsNullOrEmpty(barcode))
            _vm.AddBarcode(barcode);
    }

    // Swipe → Sil
    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is BarcodeVm barcode)
        {
            _vm.RemoveItem(barcode);
        }
    }

    // Kaydet
    private void OnSaveClicked(object sender, EventArgs e)
    {
        _ = SafeSaveAsync();
    }

    private async Task SafeSaveAsync()
    {
        if (_vm.IsBusy)
            return;

        if (_vm.Barcodes.Count == 0)
        {
            await DisplayAlert(
                "Uyarı",
                "Kaydedilecek satır yok.",
                "Tamam");
            return;
        }

        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();

        try
        {
            _vm.IsBusy = true;
            _loadingService.Show("Depo ve sayım fişi kaydediliyor...");
            await Task.Yield(); // UI güncellemesi için

            CountMVm countMVm = new CountMVm();
            countMVm.DocNo = _vm.CountNo;
            countMVm.CreatedAt = _vm.CreatedAt;
            countMVm.Items = _vm.Items.ToList();
            countMVm.Barcodes = _vm.Barcodes.ToList();

            var draft = CountMapper.ToModel(countMVm);
            var result = await _countService.SaveAsync(draft, _saveCts.Token);

            if (result.Success)
            {
                await DisplayAlert(
                    "Başarılı",
                    "Sayım başarıyla kaydedildi.",
                    "Tamam");

                _vm.Clear();
                _vm.Barcodes.Clear();
                _vm.Items.Clear();

                foreach (var item in result.Data.Barcodes)
                {
                    _vm.Barcodes.Add(CountMapper.ToModel(item));
                }
                foreach (var item in result.Data.Items)
                {
                    _vm.Items.Add(CountMapper.ToModel(item));
                }
            }
            else
            {
                await DisplayAlert(
                    "Hata",
                    result.Message ?? "ERP işlemi başarısız.",
                    "Tamam");
            }
        }
        catch (OperationCanceledException)
        {
            // Kullanıcı bilerek iptal ettiyse sessiz geçmek OK
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Hata",
                ex.Message,
                "Tamam");
        }
        finally
        {
            _vm.IsBusy = false;
            _loadingService.Hide();
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
