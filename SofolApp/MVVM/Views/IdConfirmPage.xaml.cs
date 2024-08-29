using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class IdConfirmPage : ContentPage
    {
        public IdConfirmPage(IdConfirmPageVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            NavigateBack();
            return true;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            ((IdConfirmPageVM)BindingContext).LoadUserStatusCommand.Execute(null);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync(nameof(CreditPage));
        }
    }
}
