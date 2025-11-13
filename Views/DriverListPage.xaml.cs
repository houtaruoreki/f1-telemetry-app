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

        // Populate year picker with F1 seasons (2024 back to 2018)
        // OpenF1 API has complete data from 2018 onwards
        for (int year = 2024; year >= 2018; year--)
        {
            YearPicker.Items.Add(year.ToString());
        }
        YearPicker.SelectedIndex = 0; // Default to 2024
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
