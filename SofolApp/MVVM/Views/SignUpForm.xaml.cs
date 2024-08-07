using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpForm : ContentPage
    {
        public SignUpForm(SignUpVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//SignInForm");
            });
            return true;
        }
    }
}