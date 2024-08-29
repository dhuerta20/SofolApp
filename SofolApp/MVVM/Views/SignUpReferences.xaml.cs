using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpReferences : ContentPage
    {
        private readonly SignUpReferencesVM _viewModel;

        public SignUpReferences(SignUpReferencesVM viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.ReturnToRegisterCommand.ExecuteAsync(null);
            return true;
        }
    }
}