using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpImg : ContentPage
    {
        public SignUpImg(SignUpImgVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as SignUpImgVM).InitializeCommand.ExecuteAsync(null);
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