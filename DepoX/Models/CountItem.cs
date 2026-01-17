using CommunityToolkit.Mvvm.ComponentModel;

namespace DepoX.Models;

/// <summary>
/// Sayım satırı. ObservableObject sayesinde satır içi alanlar değişince UI otomatik güncellenir.
/// (Örn: var existing = ...; existing.Quantity++;
///  gibi durumlarda eski haliyle UI güncellenmiyordu.)
/// </summary>
public partial class CountItem : ObservableObject
{
    [ObservableProperty]
    private string barcode = string.Empty;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private string color = string.Empty;

    [ObservableProperty]
    private string quality = string.Empty;
}