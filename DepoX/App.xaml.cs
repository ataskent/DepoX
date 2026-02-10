using System.Runtime.Versioning;

namespace DepoX
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;

        public App(AppShell appShell)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
            _appShell = appShell;
        }

        [SupportedOSPlatform("windows10.0.17763.0")]
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }
    }
}

