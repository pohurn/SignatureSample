using SignatureApp.Views;

namespace SignatureApp
{
    public partial class App : Application
    {
        public static NavigationPage np;

        public App()
        {
            InitializeComponent();

            np = new NavigationPage(new SignaturePage());

            MainPage = np;
        }
    }
}
