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
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync(nameof(SignInForm));
            });
            return true;
        }
    }
}