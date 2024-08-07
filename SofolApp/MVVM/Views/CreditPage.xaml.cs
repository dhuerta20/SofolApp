using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class CreditPage : ContentPage
    {
        public CreditPage(CreditPageVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;         
        }
 
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is CreditPageVM viewModel)
            {
                await viewModel.LoadUserData();
            }
        }
    }
}
