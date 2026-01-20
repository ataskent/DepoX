using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DepoX.Features.Count;

public class CountViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public Guid ClientDraftId { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ObservableCollection<CountItemVm> Items { get; } = new();


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



    public void AddBarcode(string barcode)
    {
        var existing = Items.FirstOrDefault(x => x.Barcode == barcode);

        if (existing != null)
        {
            existing.Quantity += 1;
        }
        else
        {
            Items.Add(new CountItemVm
            {
                Barcode = barcode,
                Quantity = 1
            });
        }
    }
    public void RemoveItem(CountItemVm item)
    {
        if (Items.Contains(item))
            Items.Remove(item);
    }

}
public class CountItemVm
{
    public string Barcode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    // UI'ya özel
    public bool IsEditing { get; set; }
    public bool IsSelected { get; set; }
}



