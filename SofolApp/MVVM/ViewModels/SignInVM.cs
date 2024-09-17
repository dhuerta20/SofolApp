using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.Services;
using Sentry;
using SofolApp.MVVM.Views;
using Firebase.Auth;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignInVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isLoading;

        public SignInVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowAlert("Error", "Por favor, ingrese un email y contraseña");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await ShowAlert("Error", "Por favor, ingrese un email válido");
                return;
            }

            if (!IsValidPassword(Password))
            {
                await ShowAlert("Error de Contraseña", "La contraseña debe tener al menos 8 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales");
                return;
            }

            IsLoading = true;

            try
            {
                SentrySdk.AddBreadcrumb("Attempting to sign in", "info");
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.SetTag("email", Email);
                });

                var credentials = await _firebaseConnection.SignInAsync(Email, Password);
                var currentUser = credentials.User;

                if (currentUser != null)
                {
                    var userEmail = currentUser.Info.Email;
                    await SecureStorage.SetAsync("userEmail", userEmail);
                    await Shell.Current.GoToAsync(nameof(CreditPage));
                    Console.WriteLine("Navigation to CreditApp completed");
                }
                else
                {
                    await ShowAlert("Error", "No se pudo obtener la información del usuario");
                }
            }
            catch (FirebaseAuthException authEx)
            {
                SentrySdk.CaptureException(authEx);
                await HandleFirebaseAuthException(authEx);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await ShowAlert("Error", "Ocurrió un error inesperado. Por favor, intente de nuevo más tarde.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task HandleFirebaseAuthException(FirebaseAuthException authEx)
        {
            switch (authEx.Reason)
            {
                case AuthErrorReason.InvalidEmailAddress:
                    await ShowAlert("Error de Email", "La dirección de email no es válida");
                    break;
                case AuthErrorReason.WrongPassword:
                    await ShowAlert("Error de Contraseña", "La contraseña es incorrecta");
                    break;
                case AuthErrorReason.UserNotFound:
                    await ShowAlert("Error de Usuario", "No se encontró ningún usuario con este email");
                    break;
                case AuthErrorReason.TooManyAttemptsTryLater:
                    await ShowAlert("Demasiados Intentos", "Ha realizado demasiados intentos. Por favor, intente más tarde");
                    break;
                default:
                    await ShowAlert("Error de Autenticación", "Ocurrió un error durante la autenticación. Por favor, intente de nuevo");
                    break;
            }
        }

        [RelayCommand]
        private async Task NavigateToForgotPassword()
        {
            await Shell.Current.GoToAsync(nameof(ForgotPass));
        }

        [RelayCommand]
        private async Task NavigateToSignUp()
        {
            await Shell.Current.GoToAsync(nameof(SignUpForm));
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

        private async Task ShowAlert(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }
    }
}