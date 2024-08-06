using SofolApp.MVVM.Views;

namespace SofolApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            // Breakpoint aquí
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AppShell constructor: {ex}");
                throw;
            }
        }
    }
}
