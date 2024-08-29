using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views;

public partial class ForgotPass : ContentPage
{
	public ForgotPass(ForgotPassVM viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}