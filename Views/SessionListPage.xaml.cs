namespace F1TelemetryApp.Views;

using F1TelemetryApp.ViewModels;

/// <summary>
/// Page for displaying a list of F1 sessions.
/// </summary>
public partial class SessionListPage : ContentPage
{
    private readonly SessionListViewModel _viewModel;

    public SessionListPage(SessionListViewModel viewModel)
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

        // Load sessions when page appears
        if (!_viewModel.Sessions.Any())
        {
            await _viewModel.LoadSessionsAsync();
        }
    }
}
