namespace F1TelemetryApp.Views;

using F1TelemetryApp.ViewModels;

/// <summary>
/// Page for displaying a list of F1 drivers.
/// </summary>
public partial class DriverListPage : ContentPage
{
    private readonly DriverListViewModel _viewModel;

    public DriverListPage(DriverListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Populate year picker with last 5 years
        var currentYear = DateTime.UtcNow.Year;
        for (int i = 0; i < 5; i++)
        {
            YearPicker.Items.Add((currentYear - i).ToString());
        }
        YearPicker.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load drivers when page appears
        if (!_viewModel.Drivers.Any())
        {
            await _viewModel.LoadDriversAsync();
        }
    }
}
