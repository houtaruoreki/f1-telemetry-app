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

        // Load sessions when page appears
        if (!_viewModel.Sessions.Any())
        {
            await _viewModel.LoadSessionsAsync();
        }
    }
}
