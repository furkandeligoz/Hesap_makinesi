

namespace Hesap_makinesi.Ekranlar;

public partial class Ayarlar : ContentPage
{
    public Ayarlar()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        switch (Preferences.Default.Get("app_theme", "System"))
        {
            case "Light": LightRadio.IsChecked = true; break;
            case "Dark": DarkRadio.IsChecked = true; break;
            default: SystemRadio.IsChecked = true; break;
        }
    }

    private void OnThemeChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not RadioButton radio || radio.Value is not string theme) return;

        Preferences.Default.Set("app_theme", theme);

        Application.Current!.UserAppTheme = theme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified   
        };
    }
}

