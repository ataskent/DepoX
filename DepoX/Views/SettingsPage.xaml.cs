using DepoX.Features.Shared;

namespace DepoX.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SharedViewModel _vm;
    private readonly ISharedService _sharedService;
    private CancellationTokenSource? _saveCts;

    public SettingsPage(
        SharedViewModel vm,
        ISharedService sharedService)
    {
        InitializeComponent();

        _vm = vm;
        _sharedService = sharedService;
        BindingContext = _vm;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _vm.LoadInitialAsync();
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _saveCts?.Cancel();
    }

    // ===============================
    // BASKET SEÇÝMÝ
    // ===============================

    private void OnBranchSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is BranchItemVm branch)
        {
            _vm.SelectBranch(branch);
        }
    }

    private void OnCloseBranchPicker(object sender, EventArgs e)
    {
        _vm.CloseBranchPicker();
    }


    // ===============================
    // DEPO SEÇÝMÝ
    // ===============================

    private void OnWhouseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is WhouseItemVm whouse)
        {
            _vm.SelectWhouse(whouse);
        }
    }

    private void OnCloseWhousePicker(object sender, EventArgs e)
    {
        _vm.CloseWhousePicker();
    }

}