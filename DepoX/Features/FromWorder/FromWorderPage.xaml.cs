using DepoX.Features.Basket;

namespace DepoX.Features.FromWorder;

public partial class FromWorderPage : ContentPage
{
    private readonly FromWorderViewModel _vm;
    private readonly IFromWorderService _fromWorderService;
    private CancellationTokenSource? _saveCts;

    public FromWorderPage(
        FromWorderViewModel vm,
        IFromWorderService fromWorderService)
    {
        InitializeComponent();

        _vm = vm;
        _fromWorderService = fromWorderService;
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

    private void OnTransferSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TransferMVm transfer)
        {
            _vm.SelectTransfer(transfer);
        }
    }

    private void OnCloseTransferPicker(object sender, EventArgs e)
    {
        _vm.CloseTransferPicker();
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
            swipe.CommandParameter is BarcodeVm item)
        {
            _vm.RemoveItem(item);
        }
    }

    // ===============================
    // KAYDET
    // ===============================


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