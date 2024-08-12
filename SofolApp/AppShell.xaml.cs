using SofolApp.MVVM.Views;

namespace SofolApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                InitializeComponent();
                }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AppShell constructor: {ex}");
                throw;
            }

            Routing.RegisterRoute("SignInForm", typeof(SignInForm));
            Routing.RegisterRoute("SignUpImg", typeof(SignUpImg));
            Routing.RegisterRoute("SignUpReferences", typeof(SignUpReferences));
            Routing.RegisterRoute("SignUpForm", typeof(SignUpForm));
            Routing.RegisterRoute("CreditApp", typeof(CreditPage));
            Routing.RegisterRoute("IdConfirm", typeof(IdConfirmPage));
            Routing.RegisterRoute("PersonalData", typeof(PersonalDataPage));
            Routing.RegisterRoute("Status", typeof(ReferencesPage));
            //Routing.RegisterRoute("ForgotPass", typeof(ForgotPass));
        }
    }
}
