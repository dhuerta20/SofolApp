namespace SofolApp.MVVM.Views;

public partial class RegisterForm_RegisterPage : ContentPage
{
	public RegisterForm_RegisterPage()
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void Next_RegisterPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}