using DepoX.Dtos;
using DepoX.Features.Basket;
using DepoX.Features.Count;
using DepoX.Services.Dialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DepoX.Features.Count;

public class CountViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly ICountService _countService;
    private readonly ILoadingService _loadingService;
    private readonly IDialogService _dialogService;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public Guid ClientDraftId { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ObservableCollection<ItemVm> Items { get; } = new();
    public ObservableCollection<BarcodeVm> Barcodes { get; } = new();

    public ObservableCollection<WhouseVm> Whouses { get; } = new();
    public ObservableCollection<CountVm> Counts { get; } = new();



    public async void CreateNewCount()
    {
        await CreateNewCountAsync();
    }

    public async Task CreateNewCountAsync()
    {
        var confirm = await _dialogService.ShowConfirmAsync(
            "Yeni Kayıt",
            "Yeni sayım fişi oluşturulacak. Devam etmek istiyor musunuz?"
        );

        if (!confirm)
            return;

        ClientDraftId = Guid.NewGuid();
        CreatedAt = DateTime.Now;

        CountNo = $"{DateTime.Now:yyMMddHHmmssfff}";
        SelectedWhouse = null;

        Items.Clear();
        Barcodes.Clear();

        RefreshCommands();
    }


    public void ClearCount()
    {
        Items.Clear();
        Barcodes.Clear();
    }

    public async void DeleteCount()
    {
        await DeleteCountAsync(CountNo);

        RefreshCommands();
    }

    void RefreshCommands()
    {
        (ClearCountCommand as Command)?.ChangeCanExecute();
        (DeleteCountCommand as Command)?.ChangeCanExecute();
    }

    public ICommand NewCountCommand { get; }
    public ICommand ClearCountCommand { get; }
    public ICommand DeleteCountCommand { get; }
    public ICommand OpenCountListCommand { get; }
    public ICommand OpenWhouseListCommand { get; }

    public CountViewModel(ICountService countService, IDialogService dialogService, ILoadingService loadingService)
    {
        _countService = countService;
        _dialogService = dialogService;
        _loadingService = loadingService;

        NewCountCommand = new Command(CreateNewCount);
        ClearCountCommand = new Command(ClearCount, () => HasActiveCount);
        DeleteCountCommand = new Command(DeleteCount, () => HasActiveCount);
        OpenCountListCommand = new Command(OpenCountList);
        OpenWhouseListCommand = new Command(OpenWhouseList);

        // Sayfa yüklendiğinde depo listesini yükle
        //_ = LoadWhousesAsync();
    }

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



    CountVm? _selectedCount;
    public CountVm? SelectedCount
    {
        get => _selectedCount;
        set
        {
            _selectedCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanScanBarcode));
            OnPropertyChanged(nameof(SelectedCountName));
        }
    }

    public string SelectedCountName => SelectedCount?.Code ?? "";


    string _countNo = string.Empty;
    public string CountNo
    {
        get => _countNo;
        set
        {
            if (_countNo == value) return;
            _countNo = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasActiveCount));
            OnPropertyChanged(nameof(IsOpen));
            OnPropertyChanged(nameof(CanScanBarcode));
            RefreshCommands();
        }
    }    

    public bool HasActiveCount => !string.IsNullOrWhiteSpace(CountNo);
    public bool IsOpen => HasActiveCount;

    // Barkod okutma şartı
    public bool CanScanBarcode =>
        HasActiveCount && SelectedWhouse != null;

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
    // ?? Count PICKER
    // =========================

    bool _isCountPickerOpen;
    public bool IsCountPickerOpen
    {
        get => _isCountPickerOpen;
        set
        {
            if (_isCountPickerOpen == value) return;
            _isCountPickerOpen = value;
            OnPropertyChanged();
        }
    }


    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value)
                return;

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

    public void Clear()
    {
        Items.Clear();
        ClientDraftId = Guid.NewGuid();
    }



    public void AddBarcode(string barcode)
    {
        var existing = Barcodes.FirstOrDefault(x => x.Code == barcode);

        if (existing != null)
        {
            existing.Quantity += 1;
        }
        else
        {
            Barcodes.Add(new BarcodeVm
            {
                Code = barcode,
                Quantity = 1
            });
        }
    }
    public void RemoveItem(BarcodeVm barcode)
    {
        var existing = Barcodes.FirstOrDefault(x => x.Code == barcode.Code);
        if (existing != null)
            Barcodes.Remove(existing);
    }


    public async Task LoadInitialAsync()
    {
        if (IsBusy)
            return;

        try
        {
            _loadingService.Show("Depo ve sayım fişi listesi yükleniyor...");
            await Task.Yield(); // UI güncellemesi için

            IsBusy = true;

            var result = await _countService.GetCountAsync();

            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return;
            }

            Whouses.Clear();
            foreach (var w in result.Data!.Whouses)
                Whouses.Add(CountMapper.ToModel(w));

            Counts.Clear();
            foreach (var b in result.Data!.Counts)
                Counts.Add(CountMapper.ToModel(b));

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Depo listesi yüklenirken hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            _loadingService.Hide();
        }
    }

    void OpenCountList()
    {
        IsCountPickerOpen = true;
    }

    void OpenWhouseList()
    {
        if (!IsOpen)
        {
            ErrorMessage = "Önce sayım fişi seçin.";
            return;
        }

        IsWhousePickerOpen = true;
    }

    public void SelectWhouse(WhouseVm whouse)
    {
        SelectedWhouse = whouse;
        IsWhousePickerOpen = false;
    }

    public void SelectCount(CountVm count)
    {
        SelectedCount = count;
        CountNo = count.Code;
        IsCountPickerOpen = false;

        var Task = LoadCountItemsAsync(CountNo);
    }

    public async Task DeleteCountAsync(string countCode)
    {
        try
        {
            IsBusy = true;

            var confirm = await _dialogService.ShowConfirmAsync(
                                   "Uyarı",
                                   "Sayım fişi silinecek. Devam etmek istiyor musunuz?"
                               );

            if (!confirm)
                return;

            _loadingService.Show("Depo ve sayım fişi siliniyor...");
            await Task.Yield(); // UI güncellemesi için

            var erpResult = await _countService
                .DeleteCountDataAsync(countCode);


            if (erpResult?.Data == null)
            {
                await _dialogService.ShowAlertAsync("Uyarı", "Servisten bilgi alınamadı.");
                return;
            }

            if(erpResult.Success)
            {

                await _dialogService.ShowAlertAsync("Uyarı", "Sayım Fişi Silindi");
                
                Items.Clear();
                Barcodes.Clear();

                CountNo = null;
                SelectedWhouse = null;
                ClientDraftId = Guid.NewGuid();
            }
            else
            {
                await _dialogService.ShowAlertAsync("Hata", erpResult.Message);
            }

            

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Sayım fişi silinirken hata: {ex.Message}";
            await _dialogService.ShowAlertAsync("Hata", ErrorMessage);

        }
        finally
        {
            IsBusy = false;
            _loadingService.Hide();
        }
    }

    public async Task LoadCountItemsAsync(string countCode)
    {
        try
        {
            IsBusy = true;

            _loadingService.Show("Depo ve sayım fişi yükleniyor...");
            await Task.Yield(); // UI güncellemesi için

            var erpResult = await _countService
                .GetCountDataAsync(countCode);

            Items.Clear();
            Barcodes.Clear();

            if (erpResult?.Data == null)
                return;

            CountNo = erpResult.Data.DocNo;
            CreatedAt = erpResult.Data.CreatedAt;
            SelectedWhouse = Whouses.Select(x => x).FirstOrDefault(x => x.Code == erpResult.Data.WhouseCode);

            foreach (var dto in erpResult.Data.Items)
                Items.Add(CountMapper.ToModel(dto));
            foreach (var dto in erpResult.Data.Barcodes)
                Barcodes.Add(CountMapper.ToModel(dto));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Sayım fişi yüklenirken hata: {ex.Message}";

        }
        finally
        {
            IsBusy = false;
            _loadingService.Hide();
        }
    }

    public void CloseWhousePicker()
    {
        IsWhousePickerOpen = false;
    }

    public void CloseCountPicker()
    {
        IsCountPickerOpen = false;
    }



}



