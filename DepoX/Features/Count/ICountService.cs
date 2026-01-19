using System.Collections.ObjectModel;

namespace DepoX.Features.Count;

public interface ICountService
{
    /// <summary>
    /// Terminalde okutulan tüm barkodları ERP’ye tek seferde gönderir.
    /// Barkod çözümleme ve doğrulama ERP tarafında yapılır.
    /// </summary>
    Task SaveDraftAsync(
        CountDraftDto draft,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Barkodu hiçbir doğrulama yapmadan sepete ekler.
    /// Aynı barkod varsa sadece adedini artırır.
    /// </summary>
    void AddByBarcode(
        ObservableCollection<CountItemModel> basket,
        string barcode);
    void RemoveByBarcode(
        ObservableCollection<CountItemModel> basket,
        CountItemModel item);
}
