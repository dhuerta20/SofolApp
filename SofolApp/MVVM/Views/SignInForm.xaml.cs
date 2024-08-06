using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;
using System;
using System.Text.RegularExpressions;
using Firebase.Auth;

namespace SofolApp.MVVM.Views
{
    public partial class SignInForm : ContentPage
    {
        public SignInForm()
        {
            InitializeComponent();
        }

        private async void SignInBtn(object sender, EventArgs e)
        {
            string email = emailEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Por favor, ingrese un email y contraseña", "OK");
                return;
            }

            if (!IsValidEmail(email))
            {
                await DisplayAlert("Error", "Por favor, ingrese un email válido", "OK");
                return;
            }

            if (!IsValidPassword(password))
            {
                await DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales", "OK");
                return;
            }

            try
            {
                FirebaseConnection firebaseConnection = new FirebaseConnection();
                var credentials = await firebaseConnection.SignInAsync(email, password);

                // Obtén el correo electrónico del usuario autenticado
                var currentUser = credentials.User;
                if (currentUser != null)
                {
                    var userEmail = currentUser.Info.Email;
                    // Guarda el email en SecureStorage si es necesario
                    await SecureStorage.SetAsync("userEmail", userEmail);
                }

                await Shell.Current.GoToAsync("//CreditApp");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void NavToForgotPass(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ForgotPass");
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private async void NavToSignUp(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpForm");
        }
    }
}

