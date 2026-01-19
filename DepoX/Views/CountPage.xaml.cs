using DepoX.Features.Count;

namespace DepoX.Views;

public partial class CountPage : ContentPage
{
    public CountViewModel VM => BindingContext as CountViewModel;
    private readonly ICountService _countService;

    public CountPage(ICountService countService)
    {
        InitializeComponent();
        _countService = countService;
    }


    private void OnBarcodeEntered(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(barcode))
            return;

        _countService.AddByBarcode(VM.CountItems, barcode);

        BarcodeEntry.Text = string.Empty;
        BarcodeEntry.Focus();
    }
    private void OnDeleteItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is CountItemModel item)
        {
            _countService.RemoveByBarcode(VM.CountItems, item);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        VM.IsEditing = false;

        if (VM.CountItems.Count == 0)
        {
            await DisplayAlert("Uyarı", "En az 1 barkod okutmalısın.", "Tamam");
            return;
        }

        var draft = new CountDraftDto
        {
            ClientDraftId = Guid.NewGuid().ToString(),
            WorkplaceCode = "01",

            Items = VM.CountItems
                .Select(x => new CountDraftItemDto
                {
                    Barcode = x.Barcode,
                    Quantity = x.Quantity,

                    // sözleşmede dursun ama terminal doldurmaz
                    StockCode = string.Empty,
                    FromWarehouseCode = string.Empty,
                    LocationCode = string.Empty
                })
                .ToList()
        };

        try
        {
            await _countService.SaveDraftAsync(draft);
            await DisplayAlert("OK", "Barkodlar ERP'ye gönderildi.", "Tamam");

            VM.CountItems.Clear();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }
}
