using SofolApp.MVVM.ViewModels;
using SofolApp.MVVM.Models;

namespace SofolApp.MVVM.Views
{
    public partial class PersonalDataPage : ContentPage
    {
        private readonly PersonalDataPageVM _viewModel;

        public PersonalDataPage(PersonalDataPageVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = viewModel.GoBackCommand
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeCommand.ExecuteAsync(null);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.GoBackCommand.Execute(null);
            return true;
        }
    }
}