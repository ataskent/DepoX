using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace DepoX.Features.Basket;

public class BasketViewModel : INotifyPropertyChanged
{
    private readonly IBasketService _basketService;

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // =========================
    // ?? SEPET
    // =========================

    string? _basketNo;
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

    // Barkod okutma þartý
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
    // ?? LÝSTELER
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

    public BasketViewModel(IBasketService basketService)
    {
        _basketService = basketService;

        NewBasketCommand = new Command(CreateNewBasket);
        ClearBasketCommand = new Command(ClearBasket, () => HasActiveBasket);
        DeleteBasketCommand = new Command(DeleteBasket, () => HasActiveBasket);
        OpenBasketListCommand = new Command(OpenBasketList);
        OpenWhouseListCommand = new Command(OpenWhouseList);

        // Sayfa yüklendiðinde depo listesini yükle
        //_ = LoadWhousesAsync();
    }

    // =========================
    // ?? WHOUSE ÝÞLEMLERÝ
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
            ErrorMessage = "Önce sepet oluþturun.";
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
            Items.Add(BasketMapper.ToModel(dto));
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
    // ?? SEPET ÝÞLEMLERÝ
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

        BasketNo = $"SB-{DateTime.Now:yyMMddHHmmssfff}";
        SelectedWhouse = null;

        Items.Clear();
        ValidatedStocks.Clear();

        RefreshCommands();
    }

    public void ClearBasket()
    {
        Items.Clear();
        ValidatedStocks.Clear();
    }

    public void DeleteBasket()
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

public class BasketData
{
    public List<WhouseVm> Whouses { get; set; } = new();
    public List<BasketVm> Baskets { get; set; } = new();
}

public class WhouseVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class BasketVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class BasketItemVm : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    string _barcode = "";
    public string Barcode
    {
        get => _barcode;
        set { _barcode = value; OnPropertyChanged(); }
    }

    decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); }
    }

    string _whouse = "";
    public string Whouse
    {
        get => _whouse;
        set { _whouse = value; OnPropertyChanged(); }
    }


    public bool IsInvalid { get; set; }
}
    


//public class BasketItemVm : INotifyPropertyChanged
//{
//    public event PropertyChangedEventHandler? PropertyChanged;
//    void OnPropertyChanged([CallerMemberName] string? name = null)
//        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

//    string _barcode = "";
//    public string Barcode
//    {
//        get => _barcode;
//        set { _barcode = value; OnPropertyChanged(); }
//    }

//    decimal _quantity;
//    public decimal Quantity
//    {
//        get => _quantity;
//        set { _quantity = value; OnPropertyChanged(); }
//    }

//    public bool IsInvalid { get; set; }
//}

public class ValidatedStockVm
{
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public decimal Quantity { get; set; }
    public bool HasError { get; set; }
}
