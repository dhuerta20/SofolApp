using Microsoft.Maui.Controls;
using SofolApp.MVVM.Views;

namespace SofolApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Inicializa la página principal
            MainPage = new NavigationPage(new RegisterPage());
        }
    }
}
