// DepoX.Models
using CommunityToolkit.Mvvm.ComponentModel;

namespace DepoX.Models;

public partial class CountItem : ObservableObject
{
    [ObservableProperty] private string barcode = string.Empty;
    [ObservableProperty] private string stockCode = string.Empty;
    [ObservableProperty] private decimal quantity;
    [ObservableProperty] private string color = string.Empty;
    [ObservableProperty] private string quality = string.Empty;
}
