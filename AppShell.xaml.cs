namespace F1TelemetryApp;

using F1TelemetryApp.Helpers;
using F1TelemetryApp.Views;

/// <summary>
/// Application shell providing navigation structure and routing.
/// </summary>
public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for detail pages (not in TabBar)
		Routing.RegisterRoute(Constants.Routes.SessionDetail, typeof(SessionDetailPage));
		Routing.RegisterRoute(Constants.Routes.Telemetry, typeof(TelemetryPage));
	}
}
