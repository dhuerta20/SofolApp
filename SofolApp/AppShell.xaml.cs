using SofolApp.MVVM.Views;

namespace SofolApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(SignInForm), typeof(SignInForm));
            Routing.RegisterRoute(nameof(SignUpImg), typeof(SignUpImg));
            Routing.RegisterRoute(nameof(SignUpReferences), typeof(SignUpReferences));
            Routing.RegisterRoute(nameof(SignUpForm), typeof(SignUpForm));
            Routing.RegisterRoute(nameof(CreditPage), typeof(CreditPage));
            Routing.RegisterRoute(nameof(IdConfirmPage), typeof(IdConfirmPage));
            Routing.RegisterRoute(nameof(PersonalDataPage), typeof(PersonalDataPage));
            Routing.RegisterRoute(nameof(ReferencesPage), typeof(ReferencesPage));
            Routing.RegisterRoute(nameof(ForgotPass), typeof(ForgotPass));
        }
    }
}