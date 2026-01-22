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

        try
        {
            await _vm.LoadAsync(barcode);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

    private void OnAddSplitClicked(object sender, EventArgs e)
        => _vm.StartNewSplit();

    private void OnConfirmDraft(object sender, EventArgs e)
        => _vm.ConfirmDraft();

    private void OnCancelDraft(object sender, EventArgs e)
        => _vm.CancelDraft();

    //private void OnEditSwipe(object sender, EventArgs e)
    //{
    //    if (sender is SwipeItem swipe &&
    //        swipe.CommandParameter is SplitRowVm row)
    //        _vm.Edit(row);
    //}

    //private void OnEditDone(object sender, EventArgs e)
    //{
    //    if (sender is Button btn &&
    //        btn.CommandParameter is SplitRowVm row)
    //        _vm.FinishEdit(row);
    //}

    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is SplitRowVm row)
            _vm.Remove(row);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            await _vm.SaveAsync();

            await DisplayAlert("Başarılı", "Barkod parçalama kaydedildi.", "Tamam");

            if (!string.IsNullOrEmpty(_vm.OriginalBarcode))
                await _vm.LoadAsync(_vm.OriginalBarcode);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }
}
