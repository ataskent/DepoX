namespace DepoX.Features.Count;

public partial class CountPage : ContentPage
{
    public CountPage(CountViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // 🔹 Barkod okutulduğunda (ENTER)
    private void OnBarcodeCompleted(object sender, EventArgs e)
    {
        if (BindingContext is not CountViewModel vm)
            return;

        var barcode = BarcodeEntry.Text?.Trim();

        BarcodeEntry.Text = string.Empty;
        BarcodeEntry.Focus();

        if (!string.IsNullOrEmpty(barcode))
            vm.AddBarcode(barcode);
    }

    // 🔹 Swipe → Sil
    private void OnSwipeDelete(object sender, EventArgs e)
    {
        if (sender is not SwipeItem swipe)
            return;

        if (swipe.CommandParameter is CountItemDto item &&
            BindingContext is CountViewModel vm)
        {
            vm.RemoveItem(item);
        }
    }
}
