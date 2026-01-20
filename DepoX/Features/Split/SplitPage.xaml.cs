namespace DepoX.Features.Split;

public partial class SplitPage : ContentPage
{
    private readonly SplitViewModel _vm;

    private readonly ISplitService _splitService;
    private CancellationTokenSource? _cts;

    public SplitPage(
    SplitViewModel vm,
    ISplitService splitService)
    {
        InitializeComponent();
        _vm = vm;
        _splitService = splitService;
        BindingContext = _vm;
    }

    //// Barkod okutulunca
    //private void OnBarcodeCompleted(object sender, EventArgs e)
    //{
    //    var code = BarcodeEntry.Text?.Trim();
    //    BarcodeEntry.Text = "";
    //    BarcodeEntry.Focus();

    //    if (string.IsNullOrEmpty(code))
    //        return;

    //    // ÞÝMDÝLÝK MOCK – ERP’den gelecek
    //    _vm.SetOriginal(new SplitRowVm
    //    {
    //        StockCode = "STK-001",
    //        LotCode = "P23",
    //        ColorCode = "KIRMIZI",
    //        UnitCode = "ADET",
    //        Quantity = 10
    //    });
    //}
    private async void OnBarcodeCompleted(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text?.Trim();
        BarcodeEntry.Text = "";
        BarcodeEntry.Focus();

        if (string.IsNullOrEmpty(barcode))
            return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            var dto = await _splitService.GetBarcodeAsync(barcode, _cts.Token);
            await _vm.LoadFromErpAsync(dto);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }


    private void OnAddSplitClicked(object sender, EventArgs e)
        => _vm.AddNewSplit();

    private void OnEditSwipe(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is SplitRowVm row)
        {
            _vm.Edit(row);
        }
    }

    private void OnEditDone(object sender, EventArgs e)
    {
        if (sender is Button btn &&
            btn.CommandParameter is SplitRowVm row)
        {
            _vm.FinishEdit(row);
        }
    }

    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is SplitRowVm row)
        {
            _vm.Remove(row);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Kaydet",
            $"Yeni: {_vm.NewSplits.Count}, Eski: {_vm.ExistingSplits.Count}",
            "Tamam");
    }
}
