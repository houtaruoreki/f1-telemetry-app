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

        // Populate year picker with last 5 years starting from 2024
        for (int i = 0; i < 5; i++)
        {
            YearPicker.Items.Add((2024 - i).ToString());
        }
        YearPicker.SelectedIndex = 0;

        // Handle year picker selection changes
        YearPicker.SelectedIndexChanged += (s, e) =>
        {
            if (YearPicker.SelectedIndex >= 0)
            {
                var selectedYear = int.Parse(YearPicker.Items[YearPicker.SelectedIndex]);
                _viewModel.SelectedYear = selectedYear;
            }
        };
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
