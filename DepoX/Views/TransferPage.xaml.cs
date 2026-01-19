namespace DepoX.Views;

public partial class TransferPage : ContentPage
{
    //private readonly LocalDataService _local;
    //private readonly SyncService _sync;

    public TransferPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "local.db");
        //_local = new LocalDataService(dbPath);
        //_sync = new SyncService(new HttpClient(), _local);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        //var tx = new StockTransaction
        //{
        //    ProductCode = ProductEntry.Text,
        //    FromWhouse = FromWhouseEntry.Text,
        //    ToWhouse = ToWhouseEntry.Text,
        //    Quantity = int.TryParse(QuantityEntry.Text, out int qty) ? qty : 0,
        //    OperationType = "TRANSFER"
        //};

        //await _local.SaveTransactionAsync(tx);
        await DisplayAlert("Kayýt", "Transfer offline olarak kaydedildi.", "Tamam");
    }

    private async void OnSyncClicked(object sender, EventArgs e)
    {
        //await _sync.SyncAsync();
        await DisplayAlert("Senkronizasyon", "Transferler servise gönderildi.", "Tamam");
    }
}