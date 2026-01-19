using System.Collections.ObjectModel;

namespace DepoX.Features.Count;

public class CountViewModel
{
    public ObservableCollection<CountItemModel> CountItems { get; } = new();

    public bool IsEditing { get; set; }
}
