

namespace Hesap_makinesi
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string savedTheme = Preferences.Default.Get("app_theme", "System");
            UserAppTheme = savedTheme switch
            {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Title = "Hesap Makinesi",
                Width = 400,
                Height = 800,
                MinimumWidth = 300,
                MinimumHeight = 400
            };
        }
    }
}