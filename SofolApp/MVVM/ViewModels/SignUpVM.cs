using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.ViewModels;
using System.Text.RegularExpressions;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpVM : ObservableObject
    {
        private readonly FirebaseConnection _firebaseConnection;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string phone;

        [ObservableProperty]
        private string password;

        public SignUpVM()
        {
            _firebaseConnection = new FirebaseConnection();
        }

        [RelayCommand]
        private async Task SignUpAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, complete todos los campos", "OK");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, ingrese un correo válido", "OK");
                return;
            }

            if (!IsValidPassword(Password))
            {
                await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres, incluir mayúsculas, minúsculas, números y al menos un carácter especial", "OK");
                return;
            }

            if (!IsValidPhoneNumber(Phone))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, ingrese un número de teléfono válido de 10 dígitos", "OK");
                return;
            }

            try
            {
                var userCredential = await _firebaseConnection.CreateUserAsync(Email, Password, FirstName, LastName, Phone);
                if (userCredential != null && !string.IsNullOrEmpty(userCredential.User.Uid))
                {
                    await SecureStorage.SetAsync("userEmail", Email);
                    await Shell.Current.GoToAsync("//SignUpImg");
                    await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo crear el usuario. Por favor, intente de nuevo.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detallado: {ex}");
                await Shell.Current.DisplayAlert("Error", $"Error al registrar: {ex.Message}", "OK");
            }
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_\-])[A-Za-z\d@$!%*?&_\-]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private bool IsValidPhoneNumber(string phone)
        {
            string phonePattern = @"^\d{10}$";
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}