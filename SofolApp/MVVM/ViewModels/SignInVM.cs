using System;
using System.Windows.Input;
using System.Text.RegularExpressions;
using SofolApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignInVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public SignInVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, ingrese un email y contraseña", "OK");
                return;
            }
            if (!IsValidEmail(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, ingrese un email válido", "OK");
                return;
            }
            if (!IsValidPassword(Password))
            {
                await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales", "OK");
                return;
            }
            try
            {
                var credentials = await _firebaseConnection.SignInAsync(Email, Password);
                var currentUser = credentials.User;
                if (currentUser != null)
                {
                    var userEmail = currentUser.Info.Email;
                    await SecureStorage.SetAsync("userEmail", userEmail);
                }
                await Shell.Current.GoToAsync("//CreditApp");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToForgotPassword()
        {
            await Shell.Current.GoToAsync("//ForgotPass");
        }

        [RelayCommand]
        private async Task NavigateToSignUp()
        {
            await Shell.Current.GoToAsync("//SignUpForm");
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
    }
}