using Microsoft.Extensions.Logging;
using F1TelemetryApp.Services;
using F1TelemetryApp.ViewModels;
using F1TelemetryApp.Views;
using Microcharts.Maui;

namespace F1TelemetryApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMicrocharts()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register Services
		builder.Services.AddSingleton<ICacheService, CacheService>();
		builder.Services.AddHttpClient<IOpenF1ApiService, OpenF1ApiService>();

		// Register ViewModels
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<IntroViewModel>();
		builder.Services.AddTransient<SessionListViewModel>();
		builder.Services.AddTransient<SessionDetailViewModel>();
		builder.Services.AddTransient<DriverListViewModel>();
		builder.Services.AddTransient<TelemetryViewModel>();

		// Register Views/Pages
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<IntroPage>();
		builder.Services.AddTransient<SessionListPage>();
		builder.Services.AddTransient<SessionDetailPage>();
		builder.Services.AddTransient<DriverListPage>();
		builder.Services.AddTransient<TelemetryPage>();
		builder.Services.AddTransient<SettingsPage>();

		return builder.Build();
	}
}
