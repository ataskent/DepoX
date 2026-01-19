using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DepoX.Features.Count;

public class CountViewModel : INotifyPropertyChanged
{
    private readonly ICountService _service;

    public ObservableCollection<CountItemDto> Items { get; } = new();

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
            ((Command)SaveCommand).ChangeCanExecute();
        }
    }

    public ICommand SaveCommand { get; }

    public CountViewModel(ICountService service)
    {
        _service = service;

        SaveCommand = new Command(
            async () => await SaveAsync(),
            () => !IsBusy && Items.Count > 0);

        Items.CollectionChanged += (_, _) =>
            ((Command)SaveCommand).ChangeCanExecute();
    }

    // 🔹 Barkod okutulduğunda çağrılır
    public void AddBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return;

        var existing = Items.FirstOrDefault(x => x.Barcode == barcode);

        if (existing != null)
        {
            existing.Quantity += 1;
            return;
        }

        Items.Add(new CountItemDto
        {
            Barcode = barcode,
            Quantity = 1
        });
    }

    public void RemoveItem(CountItemDto item)
    {
        Items.Remove(item);
    }

    private async Task SaveAsync()
    {
        try
        {
            IsBusy = true;

            var draft = new CountDraftDto
            {
                ClientDraftId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Items = Items.ToList()
            };

            await _service.SaveAsync(draft);

            Items.Clear();

            await ShowMessage("Başarılı", "Sayım kaydedildi.");
        }
        catch (InvalidOperationException ex)
        {
            await ShowMessage("Uyarı", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowMessage("Hata", "Kayıt sırasında hata oluştu.\n" + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task ShowMessage(string title, string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert(title, message, "Tamam");
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
