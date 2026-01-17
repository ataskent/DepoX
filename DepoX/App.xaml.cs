using System.Runtime.Versioning;

namespace DepoX
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        [SupportedOSPlatform("windows10.0.17763.0")]
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
    //public partial class App : Application
    //{
    //    public App()
    //    {
    //        InitializeComponent();

    //        MainPage = new AppShell(); // Menü yapısına geçiyoruz
    //    }
    //}
}