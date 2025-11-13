namespace F1TelemetryApp.Views;

using F1TelemetryApp.ViewModels;

/// <summary>
/// Page for displaying detailed information about a specific session.
/// </summary>
public partial class SessionDetailPage : ContentPage
{
    public SessionDetailPage(SessionDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
