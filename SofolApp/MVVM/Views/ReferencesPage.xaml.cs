using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class ReferencesPage : ContentPage
    {
        private readonly ReferencesPageVM _viewModel;

        public ReferencesPage(ReferencesPageVM viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadUserDataAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync(nameof(CreditPage));
            });
            return true;
        }
    }
}