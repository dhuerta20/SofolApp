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
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//SignUpForm");
            });
            return true;
        }
    }
}