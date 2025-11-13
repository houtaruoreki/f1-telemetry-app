namespace F1TelemetryApp.Views;

using F1TelemetryApp.Services;

/// <summary>
/// Page for app settings and configuration.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private readonly ICacheService _cacheService;

    public SettingsPage(ICacheService cacheService)
    {
        InitializeComponent();
        _cacheService = cacheService;
    }

    private async void OnClearCacheClicked(object sender, EventArgs e)
    {
        try
        {
            _cacheService.Clear();
            await DisplayAlert("Success", "Cache cleared successfully", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to clear cache: {ex.Message}", "OK");
        }
    }
}
