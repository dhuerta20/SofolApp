using Microsoft.Maui.Controls;
using System;

namespace SofolApp.MVVM.Views
{
    public partial class RegisterForm : ContentPage
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // Aqu� debes agregar tu l�gica de autenticaci�n
                await Navigation.PushAsync(new CreditPage());
            
 
        }

        async void OnCreateAccountButtonClicked(object sender, EventArgs e)
        {
            // Navega a la p�gina RegisterPage si el usuario necesita crear una cuenta
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
