using DepoX.Features.Basket;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DepoX.Features.FromWorder;

public class FromWorderViewModel : INotifyPropertyChanged
{

    private readonly IFromWorderService _fromWorderService;

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    public ICommand OpenTransferListCommand { get; }
    public ICommand OpenWhouseListCommand { get; }
    public ICommand CloseWhousePickerCommand { get; }
    public ICommand CloseTransferPickerCommand { get; }
    public ICommand SaveTransferCommand { get; }


    public FromWorderViewModel(IFromWorderService fromWorderService)
    {
        _fromWorderService = fromWorderService;

        OpenTransferListCommand = new Command(OpenTransferList);
        OpenWhouseListCommand = new Command(OpenWhouseList);
        CloseWhousePickerCommand = new Command(CloseWhousePicker);
        CloseTransferPickerCommand = new Command(CloseTransferPicker);
        SaveTransferCommand = new Command(SaveTransferCommandExecuteAsync);


    }

    private async void SaveTransferCommandExecuteAsync(object obj)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            TransferDataVm transferDataVm = new TransferDataVm
            {
                Transfer = SelectedTransfer!,
                Barcodes = Barcodes.ToList(),
                Items = Items.ToList()
            };
            var dto = FromWorderMapper.ToModel(transferDataVm);
            var result = await _fromWorderService.SaveTransferAsync(dto);
            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return;
            }
            // Başarılı kayıt sonrası işlemler
            Barcodes.Clear();
            Items.Clear();
            TransferNo = null;
            WhouseCode = null;
            SelectedTransfer = null;
            SelectedWhouse = null;
            
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadInitialAsync()
    {
        try
        {
           

            var result = await _fromWorderService.GetTransferAsync();

            if (!result.Success)
            {
                //ErrorMessage = result.Message;
                return;
            }

            //Whouses.Clear();
            //foreach (var w in result.Data!.whouses)
            //    Whouses.Add(BasketMapper.ToModel(w));

            Transfers.Clear();
            foreach (var b in result.Data!.Transfers)
                Transfers.Add(FromWorderMapper.ToModel(b));
            Whouses.Clear();
            foreach (var w in result.Data!.Whouses)
                Whouses.Add(FromWorderMapper.ToModel(w));

        }
        catch (Exception ex)
        {
            
        }
        finally
        {
        }
    }

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
    // ?? Transfer PICKER
    // =========================

    bool _isTransferPickerOpen;
    public bool IsTransferPickerOpen
    {
        get => _isTransferPickerOpen;
        set
        {
            if (_isTransferPickerOpen == value) return;
            _isTransferPickerOpen = value;
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

    TransferMVm? _selectedTransfer;
    public TransferMVm? SelectedTransfer
    {
        get => _selectedTransfer;
        set
        {
            _selectedTransfer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanScanBarcode));
            OnPropertyChanged(nameof(SelectedTransferName));
        }
    }
    public string SelectedWhouseName => SelectedWhouse?.Name ?? "";
    public string SelectedTransferName => SelectedTransfer?.Name ?? "";

    // Barkod okutma şartı
    public bool CanScanBarcode => SelectedWhouse != null && SelectedTransfer != null;

    // =========================
    // ?? LİSTELER
    // =========================

    public ObservableCollection<BarcodeVm> Barcodes { get; } = new();
    public ObservableCollection<ItemsVm> Items { get; } = new();

    public ObservableCollection<WhouseVm> Whouses { get; } = new();
    public ObservableCollection<TransferMVm> Transfers { get; } = new();

    string? _transferNo;
    public string? TransferNo
    {
        get => _transferNo;
        set
        {
            if (_transferNo == value) return;
            _transferNo = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasActiveTransfer));
            OnPropertyChanged(nameof(IsOpen));
            OnPropertyChanged(nameof(CanScanBarcode));
            //RefreshCommands();
        }
    }
    string? _whouseCode;
    public string? WhouseCode
    {
        get => _whouseCode;
        set
        {
            if (_whouseCode == value) return;
            _whouseCode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasActiveTransfer));
            OnPropertyChanged(nameof(IsOpen));
            OnPropertyChanged(nameof(CanScanBarcode));
        }
    }

    public bool HasActiveTransfer => !string.IsNullOrWhiteSpace(TransferNo);
    public bool IsOpen => SelectedTransfer != null;

    public void OpenTransferList()
    {
        IsTransferPickerOpen = true;
    }

    public void OpenWhouseList()
    {
        if (!IsOpen)
        {
            ErrorMessage = "Önce malzeme talebi seçin.";
            return;
        }

        IsWhousePickerOpen = true;
    }

    public void SelectWhouse(WhouseVm whouse)
    {
        SelectedWhouse = whouse;
        IsWhousePickerOpen = false;
    }

    public void SelectTransfer(TransferMVm transfer)
    {
        SelectedTransfer = transfer;
        TransferNo = transfer.Code;
        WhouseCode = transfer.WhouseCode;
        IsTransferPickerOpen = false;

        var Task = LoadDetailsAsync(TransferNo);
    }

    public async Task LoadDetailsAsync(string transferCode)
    {
        var erpResult = await _fromWorderService.GetTransferDataAsync(transferCode);

        Barcodes.Clear();
        Items.Clear();

        if (erpResult?.Data == null)
            return;

        foreach (var dto in erpResult.Data.Barcodes)
            Barcodes.Add(FromWorderMapper.ToModel(dto));
        foreach (var dto in erpResult.Data.Items)
            Items.Add(FromWorderMapper.ToModel(dto));

    }



    public void CloseWhousePicker()
    {
        IsWhousePickerOpen = false;
    }

    public void CloseTransferPicker()
    {
        IsTransferPickerOpen = false;
    }

    // =========================
    // ?? SEPET İŞLEMLERİ
    // =========================

    public void AddBarcode(string barcode)
    {
        if (!CanScanBarcode)
            return;

        var existing = Barcodes.FirstOrDefault(x => x.Code == barcode);
        if (existing != null)
            existing.Quantity += 1;
        else
            Barcodes.Add(new BarcodeVm
            {
                Code = barcode,
                WhouseCode = SelectedWhouse!.Code,
                Quantity = 1
            });
    }

    public void RemoveItem(BarcodeVm item)
    {
        if (Barcodes.Contains(item))
            Barcodes.Remove(item);
    }
}
