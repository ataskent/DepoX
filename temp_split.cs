namespace DepoX.Features.Split;

public partial class SplitPage : ContentPage
{
    private readonly SplitViewModel _vm;

    public SplitPage(SplitViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    private async void OnBarcodeCompleted(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text?.Trim();
        BarcodeEntry.Text = "";
        BarcodeEntry.Focus();

        if (string.IsNullOrEmpty(barcode))
            return;

        await _vm.LoadAsync(barcode);

        await ShowMessagesAsync();
    }

    private void OnAddSplitClicked(object sender, EventArgs e)
        => _vm.StartNewSplit();

    private void OnConfirmDraft(object sender, EventArgs e)
        => _vm.ConfirmDraft();

    private void OnCancelDraft(object sender, EventArgs e)
        => _vm.CancelDraft();

    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is SplitRowVm row)
            _vm.Remove(row);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _vm.SaveAsync();
        await ShowMessagesAsync();
    }

    // ===============================
    // UI MESSAGE HANDLING
    // ===============================

    private async Task ShowMessagesAsync()
    {
        if (_vm.HasError)
        {
            await DisplayAlert("Hata", _vm.ErrorMessage!, "Tamam");
            return;
        }

        if (_vm.HasInfo)
        {
            await DisplayAlert("Bilgi", _vm.InfoMessage!, "Tamam");
        }
    }
}

