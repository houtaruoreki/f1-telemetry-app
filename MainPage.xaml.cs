namespace F1TelemetryApp;

using F1TelemetryApp.ViewModels;

/// <summary>
/// Main page of the application providing navigation to major sections.
/// </summary>
public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
