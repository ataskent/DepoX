using DepoX.Dtos;
using DepoX.Services.Dialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DepoX.Features.Basket;

public class BasketViewModel : INotifyPropertyChanged
{
    private readonly IBasketService _basketService;

    public event PropertyChangedEventHandler? PropertyChanged; 
    private readonly IDialogService _dialog; 
    private readonly ILoadingService _loading;


    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // =========================
    // ?? SEPET
    // =========================

    string? _basketNo = string.Empty;
    public string? BasketNo
    {
        get => _basketNo;
        set
        {
            if (_basketNo == value) return;
            _basketNo = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasActiveBasket));
            OnPropertyChanged(nameof(IsOpen));
            OnPropertyChanged(nameof(CanScanBarcode));
            RefreshCommands();
        }
    }

    public bool HasActiveBasket => !string.IsNullOrWhiteSpace(BasketNo);
    public bool IsOpen => HasActiveBasket;

    public Guid ClientDraftId { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    // =========================
    // ?? WHOUSE
    // =========================

    public ObservableCollection<BasketVm> Baskets { get; } = new();

    BasketVm? _selectedBasket;
    public BasketVm? SelectedBasket
    {
        get => _selectedBasket;
        set
        {
            _selectedBasket = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanScanBarcode));
            OnPropertyChanged(nameof(SelectedBasketName));
        }
    }

    public string SelectedBasketName => SelectedBasket?.Code ?? "";


    public ObservableCollection<WhouseVm> Whouses { get; } = new();

    WhouseVm? _selectedWhouse;
    public WhouseVm? SelectedWhouse
    {
        get => _selectedWhouse;
        set
        {
            _selectedWhouse = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanScanBarcode));
            OnPropertyChanged(nameof(SelectedWhouseName));
        }
    }

    public string SelectedWhouseName => SelectedWhouse?.Name ?? "";

    // Barkod okutma şartı
    public bool CanScanBarcode =>
        HasActiveBasket && SelectedWhouse != null;

    // =========================
    // ?? WHOUSE PICKER
    // =========================

    bool _isWhousePickerOpen;
    public bool IsWhousePickerOpen
    {
        get => _isWhousePickerOpen;
        set
        {
            if (_isWhousePickerOpen == value) return;
            _isWhousePickerOpen = value;
            OnPropertyChanged();
        }
    }

    // =========================
    // ?? Basket PICKER
    // =========================

    bool _isBasketPickerOpen;
    public bool IsBasketPickerOpen
    {
        get => _isBasketPickerOpen;
        set
        {
            if (_isBasketPickerOpen == value) return;
            _isBasketPickerOpen = value;
            OnPropertyChanged();
        }
    }

    // =========================
    // ?? UI STATE
    // =========================

    bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (_errorMessage == value) return;
            _errorMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    // =========================
    // ?? LİSTELER
    // =========================

    public ObservableCollection<BasketItemVm> Items { get; } = new();
    public ObservableCollection<ValidatedStockVm> ValidatedStocks { get; } = new();

    // =========================
    // ?? COMMANDS
    // =========================

    public ICommand NewBasketCommand { get; }
    public ICommand ClearBasketCommand { get; }
    public ICommand DeleteBasketCommand { get; }
    public ICommand OpenBasketListCommand { get; }
    public ICommand OpenWhouseListCommand { get; }

    public BasketViewModel(IBasketService basketService, IDialogService dialogService, ILoadingService loadingService)
    {
        _basketService = basketService;
        _dialog = dialogService;
        _loading = loadingService;  

        NewBasketCommand = new Command(CreateNewBasket);
        ClearBasketCommand = new Command(ClearBasket, () => HasActiveBasket);
        DeleteBasketCommand = new Command(DeleteBasket, () => HasActiveBasket);
        OpenBasketListCommand = new Command(OpenBasketList);
        OpenWhouseListCommand = new Command(OpenWhouseList);

        // Sayfa yüklendiğinde depo listesini yükle
        //_ = LoadWhousesAsync();
    }

    // =========================
    // ?? WHOUSE İŞLEMLERİ
    // =========================

    public async Task LoadInitialAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var result = await _basketService.GetBasketDataAsync();

            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return;
            }

            Whouses.Clear();
            foreach (var w in result.Data!.whouses)
                Whouses.Add(BasketMapper.ToModel(w));

            Baskets.Clear();
            foreach (var b in result.Data!.baskets)
                Baskets.Add(BasketMapper.ToModel(b));

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Depo listesi yüklenirken hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    void OpenBasketList()
    { 
        IsBasketPickerOpen = true;
    }

    void OpenWhouseList()
    {
        if (!IsOpen)
        {
            ErrorMessage = "Önce sepet oluşturun.";
            return;
        }

        IsWhousePickerOpen = true;
    }

    public void SelectWhouse(WhouseVm whouse)
    {
        SelectedWhouse = whouse;
        IsWhousePickerOpen = false;
    }

    public void SelectBasket(BasketVm basket)
    {
        SelectedBasket = basket;
        BasketNo = basket.Code;
        IsBasketPickerOpen = false;

        var Task = LoadBasketItemsAsync(BasketNo);
    }

    public async Task LoadBasketItemsAsync(string basketCode)
    {
        var erpResult = await _basketService
            .GetBasketBarcodeDataAsync(basketCode);

        Items.Clear();

        if (erpResult?.Data == null)
            return;

        foreach (var dto in erpResult.Data)
        {
            if (string.IsNullOrEmpty(dto.Barcode))
            {
                ValidatedStocks.Add(BasketMapper.ToModelVS(dto));
            }
            else
            {
                Items.Add(BasketMapper.ToModel(dto));
            }
        }
    }



    public void CloseWhousePicker()
    {
        IsWhousePickerOpen = false;
    }

    public void CloseBasketPicker()
    {
        IsBasketPickerOpen = false;
    }

    // =========================
    // ?? SEPET İŞLEMLERİ
    // =========================

    public void AddBarcode(string barcode)
    {
        if (!CanScanBarcode)
            return;

        var existing = Items.FirstOrDefault(x => x.Barcode == barcode);
        if (existing != null)
            existing.Quantity += 1;
        else
            Items.Add(new BasketItemVm
            {
                Barcode = barcode,
                Whouse = SelectedWhouse!.Code,
                Quantity = 1
            });
    }

    public void RemoveItem(BasketItemVm item)
    {
        if (Items.Contains(item))
            Items.Remove(item);
    }

    public void CreateNewBasket()
    {
        ClientDraftId = Guid.NewGuid();
        CreatedAt = DateTime.Now;

        BasketNo = $"{DateTime.Now:yyMMddHHmmssfff}";
        SelectedWhouse = null;

        Items.Clear();
        ValidatedStocks.Clear();

        RefreshCommands();
    }

    //public void ClearBasket()
    //{
    //    Items.Clear();
    //    ValidatedStocks.Clear();

    //    if(string.IsNullOrEmpty(BasketNo))
    //    {
    //        ErrorMessage = "Sepet Seçiniz";
    //        return;
    //    }
    //    else if(SelectedWhouse == null || string.IsNullOrEmpty(SelectedWhouse!.Code))
    //    {
    //        ErrorMessage = "Depo Seçiniz";
    //        return;
    //    }

    //    var Task = ClearBasketAsync(BasketNo, SelectedWhouse!.Code);
    //}

    public async void ClearBasket()
    {
        // ===============================
        // ÖN KONTROLLER
        // ===============================

        if (string.IsNullOrEmpty(BasketNo))
        {
            await _dialog.ShowAlertAsync(
                "Uyarı",
                "Sepet seçiniz");
            return;
        }

        if (SelectedWhouse == null || string.IsNullOrEmpty(SelectedWhouse.Code))
        {
            await _dialog.ShowAlertAsync(
                "Uyarı",
                "Depo seçiniz");
            return;
        }

        try
        {
            // ===============================
            // LOADING AÇ
            // ===============================
            _loading.Show("Sepet boşaltılıyor...");

            // 🔴 kritik nokta
            await Task.Yield();

            var erpResult = await ClearBasketAsync(
                BasketNo,
                SelectedWhouse.Code);

            if (erpResult == null)
            {
                await _dialog.ShowAlertAsync(
                    "Hata",
                    "ERP bağlantısı kurulamadı");
                return;
            }

            if (!erpResult.Data)
            {
                await _dialog.ShowAlertAsync(
                    "Hata",
                    erpResult.Message ?? "Sepet boşaltılamadı");
                return;
            }

            // ===============================
            // BAŞARILI → UI TEMİZLE
            // ===============================

            Items.Clear();
            ValidatedStocks.Clear();

            await _dialog.ShowAlertAsync(
                "Bilgi",
                "Sepet boşaltma işlemi tamamlandı");
        }
        catch (Exception ex)
        {
            await _dialog.ShowAlertAsync(
                "Hata",
                ex.Message);
        }
        finally
        {
            // ===============================
            // LOADING KAPAT (GARANTİ)
            // ===============================
            _loading.Hide();
        }
    }

    public async Task<ErpResult<bool>> ClearBasketAsync(
    string basketCode,
    string whouseCode)
    {
        return await _basketService
            .ClearBasketDataAsync(basketCode, whouseCode);
    }


    //public async Task ClearBasketAsync(string basketCode, string whouseCode)
    //{
    //    var erpResult = await _basketService
    //        .ClearBasketDataAsync(basketCode, whouseCode);

    //    if (erpResult?.Data == null)
    //        return;

    //}

    public void DeleteBasket()
    {
       var delBasket = DeleteBasketAsync();
    }

    public async Task DeleteBasketAsync()
    {
        if (string.IsNullOrEmpty(BasketNo))
        {
            await _dialog.ShowAlertAsync("Uyarı", "Sepet seçiniz");
            return;
        }

        var confirm = await _dialog.ShowConfirmAsync(
            "Onay",
            "Sepeti silmek istiyor musunuz?");

        if (!confirm) return;

        try
        {
            _loading.Show("Sepet siliniyor...");
            await Task.Yield();

            var erpResult =
                await _basketService.DeleteBasketDataAsync(BasketNo);

            if (erpResult == null || !erpResult.Data)
            {
                await _dialog.ShowAlertAsync(
                    "Hata",
                    erpResult?.Message ?? "Sepet silinemedi");
                return;
            }

            ClearBasketUi();

            await _dialog.ShowAlertAsync(
                "Başarılı",
                "İşlem tamamlandı");
        }
        finally
        {
            _loading.Hide();
        }
    }



    private void ClearBasketUi()
    {
        Items.Clear();
        ValidatedStocks.Clear();

        BasketNo = null;
        SelectedWhouse = null;
        ClientDraftId = Guid.NewGuid();

        RefreshCommands();
    }




    void RefreshCommands()
    {
        (ClearBasketCommand as Command)?.ChangeCanExecute();
        (DeleteBasketCommand as Command)?.ChangeCanExecute();
    }


}

