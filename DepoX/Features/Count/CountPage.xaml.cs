namespace DepoX.Features.Count;

public partial class CountPage : ContentPage
{
    private readonly CountViewModel _vm;
    private readonly ICountService _countService;
    private CancellationTokenSource? _saveCts;

    public CountPage(
        CountViewModel vm,
        ICountService countService)
    {
        InitializeComponent();

        _vm = vm;
        _countService = countService;
        BindingContext = _vm;
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
            swipe.CommandParameter is CountItemVm item)
        {
            _vm.RemoveItem(item);
        }
    }

    // Kaydet
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_vm.Items.Count == 0)
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

            var draft = _vm.ToModel();
            var result = await _countService.SaveAsync(draft, _saveCts.Token);

            if (result.Success)
            {
                await DisplayAlert(
                    "Başarılı",
                    "Sayım başarıyla kaydedildi.",
                    "Tamam");

                _vm.Clear();
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
            // Sessiz geçilebilir
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
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _saveCts?.Cancel();
    }
}
