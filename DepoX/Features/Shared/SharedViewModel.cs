using DepoX.Features.Basket;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DepoX.Features.Shared;

public class SharedViewModel : INotifyPropertyChanged
{

    private readonly ISharedService _sharedService;
    public SharedViewModel(ISharedService sharedService)
    {
        _sharedService = sharedService;


        OpenBranchListCommand = new Command(OpenBranchList);
        OpenWhouseListCommand = new Command(OpenWhouseList);

        // Sayfa yüklendiğinde depo listesini yükle
        //_ = LoadWhousesAsync();
    }

    void OpenBranchList()
    {
        IsBranchPickerOpen = true;
    }

    void OpenWhouseList()
    {
        IsWhousePickerOpen = true;
    }

    bool _isBranchPickerOpen;
    public bool IsBranchPickerOpen
    {
        get => _isBranchPickerOpen;
        set
        {
            if (_isBranchPickerOpen == value) return;
            _isBranchPickerOpen = value;
            OnPropertyChanged();
        }
    }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public Guid ClientDraftId { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ObservableCollection<SharedItemVm> Items { get; } = new();


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

    public void Clear()
    {
        Items.Clear();
        ClientDraftId = Guid.NewGuid();
    }


    public ICommand OpenBranchListCommand { get; }
    public ICommand OpenWhouseListCommand { get; }


    public async Task LoadInitialAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var result = await _sharedService.GetOptionalDataAsync();

            if (!result.Success)
            {
                //ErrorMessage = result.Message;
                return;
            }

            Whouses.Clear();
            foreach (var w in result.Data!.whouses)
                Whouses.Add(SharedMapper.ToModel(w));

            Branches.Clear();

            foreach (var b in result.Data!.branches)
                Branches.Add(SharedMapper.ToModel(b));

        }
        catch (Exception ex)
        {
            //ErrorMessage = $"Depo listesi yüklenirken hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }



    BranchItemVm? _selectedBranch;
    public BranchItemVm? SelectedBranch
    {
        get => _selectedBranch;
        set
        {
            _selectedBranch = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedBranchName));
        }
    }

    public string SelectedBranchName => SelectedBranch?.Code ?? "";


    WhouseItemVm? _selectedWhouse;
    public WhouseItemVm? SelectedWhouse
    {
        get => _selectedWhouse;
        set
        {
            _selectedWhouse = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedWhouseName));
        }
    }

    public string SelectedWhouseName => SelectedWhouse?.Name ?? "";

    public void SelectWhouse(WhouseItemVm whouse)
    {
        SelectedWhouse = whouse;
        IsWhousePickerOpen = false;
    }
    public void SelectBranch(BranchItemVm branch)
    {
        SelectedBranch = branch;
        IsBranchPickerOpen = false;
    }

    public void CloseBranchPicker()
    {
        SelectedBranch = null;
        IsBranchPickerOpen = false;
    }
    public void CloseWhousePicker()
    {
        SelectedWhouse = null;
        IsWhousePickerOpen = false;
    }

    public ObservableCollection<WhouseItemVm> Whouses { get; } = new();
    public ObservableCollection<BranchItemVm> Branches { get; } = new();

    public class OptionalData
    {
        public List<WhouseItemVm> Whouses { get; set; } = new();
        public List<BranchItemVm> Branches { get; set; } = new();
    }

    //public class WhouseVm
    //{
    //    public string Code { get; set; } = "";
    //    public string Name { get; set; } = "";
    //}

    //public class BranchVm
    //{
    //    public string Code { get; set; } = "";
    //    public string Name { get; set; } = "";
    //}



    //public void AddBarcode(string barcode)
    //{
    //    var existing = Items.FirstOrDefault(x => x.Barcode == barcode);

    //    if (existing != null)
    //    {
    //        existing.Quantity += 1;
    //    }
    //    else
    //    {
    //        Items.Add(new SharedItemVm
    //        {
    //            Barcode = barcode,
    //            Quantity = 1
    //        });
    //    }
    //}
    //public void RemoveItem(SharedItemVm item)
    //{
    //    if (Items.Contains(item))
    //        Items.Remove(item);
    //}

}
public class SharedItemVm
{
    public string Barcode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    // UI'ya özel
    public bool IsEditing { get; set; }
    public bool IsSelected { get; set; }
}



