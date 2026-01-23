using DepoX.Services.Erp.Dtos;

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

    //private void OnNewBarcodeClicked(object sender, EventArgs e)
    //    => _vm.StartNewBarcode();

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

    void OnOpenItemPicker(object sender, EventArgs e)
    {
        _vm.IsItemPickerOpen = true;
    }

    void OnCloseItemPicker(object sender, EventArgs e)
    {
        _vm.IsItemPickerOpen = false;
    }

    async void OnItemPicked(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ItemMetaDto item)
        {
            _vm.DraftRow!.SelectedItem = item;
            _vm.IsItemPickerOpen = false;

            await _vm.LoadLotsForSelectedItemAsync(item.Code);
        }
    }


    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ItemMetaDto item &&
            _vm.DraftRow != null)
        {
            _vm.DraftRow.SelectedItem = item;

            // 🔄 Parti listesini stoktan sonra yükle
            await _vm.LoadLotsForSelectedItemAsync(item.Code);
        }
    }


    private async void OnNewBarcodeClicked(object sender, EventArgs e)
    {
        await _vm.StartNewBarcodeAsync();
        await ShowMessagesAsync();
    }

    private async void OnConfirmDraft(object sender, EventArgs e)
    {
        if (_vm.CurrentDraftMode == DraftMode.NewBarcode)
            await _vm.ConfirmNewBarcodeAsync();
        else
            _vm.ConfirmDraft();

        await ShowMessagesAsync();
    }

    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe &&
            swipe.CommandParameter is SplitRowVm row)
        {
            _vm.Remove(row);
        }
    }

    private void OnAddSplitClicked(object sender, EventArgs e)
        => _vm.StartNewSplit();

    //private async void OnConfirmDraft(object sender, EventArgs e)
    //{
    //    if (_vm.CurrentDraftMode == DraftMode.NewBarcode)
    //        await _vm.CreateNewBarcodeAsync();
    //    else
    //        _vm.ConfirmDraft();

    //    await ShowMessagesAsync();
    //}

    private void OnCancelDraft(object sender, EventArgs e)
        => _vm.CancelDraft();

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _vm.SaveAsync();
        await ShowMessagesAsync();
    }

    private async Task ShowMessagesAsync()
    {
        if (_vm.HasError)
        {
            await DisplayAlert("Hata", _vm.ErrorMessage!, "Tamam");
            return;
        }

        if (_vm.HasInfo)
            await DisplayAlert("Bilgi", _vm.InfoMessage!, "Tamam");
    }
}
