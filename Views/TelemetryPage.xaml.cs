namespace F1TelemetryApp.Views;

using F1TelemetryApp.ViewModels;

/// <summary>
/// Page for displaying telemetry data for a specific driver.
/// </summary>
public partial class TelemetryPage : ContentPage
{
    public TelemetryPage(TelemetryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
