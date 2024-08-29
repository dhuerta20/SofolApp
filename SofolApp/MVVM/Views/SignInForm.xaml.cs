using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class SignInForm : ContentPage
    {
        public SignInForm(SignInVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    }
}