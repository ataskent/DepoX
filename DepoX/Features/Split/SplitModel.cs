using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.Split;

public class SplitBarcodeModel
{
    public string Barcode { get; set; } = "";

    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public string LotCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string UnitCode { get; set; } = "";

    public decimal Quantity { get; set; }

    public List<SplitBarcodeModel> ExistingSplits { get; set; } = new();


    public class SplitRowVm : INotifyPropertyChanged
    {
        public bool IsExisting { get; set; }

        bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing == value) return;
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNewAndNotEditing));
            }
        }

        public bool IsNewAndNotEditing => !IsExisting && !IsEditing;

        public string ItemCode { get; set; } = "";
        public string ItemName { get; set; } = "";  
        public string LotCode { get; set; } = "";
        public string ColorCode { get; set; } = "";
        public string UnitCode { get; set; } = "";
        public decimal Quantity { get; set; }

        // ERP’den doldurulacak seçim listeleri
        public IList<string> StockList { get; set; } = new List<string>();
        public IList<string> LotList { get; set; } = new List<string>();
        public IList<string> ColorList { get; set; } = new List<string>();
        public IList<string> UnitList { get; set; } = new List<string>();

        public string SummaryLine =>
            $"{ItemCode} · {ItemName} · {LotCode} · {ColorCode} · {UnitCode} · {Quantity}";

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
