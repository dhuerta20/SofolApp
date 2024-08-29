using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Views;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;

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

        public SignUpVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
        }

        [RelayCommand]
        private async Task SignUpAsync()
        {
            if (!ValidateInput())
                return;

            try
            {
                var userCredential = await _firebaseConnection.CreateUserAsync(Email, Password, FirstName, LastName, Phone);
                if (userCredential != null && !string.IsNullOrEmpty(userCredential.User.Uid))
                {
                    await SaveUserProgress(userCredential.User.Uid);
                    await Shell.Current.GoToAsync(nameof(SignUpImg));
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

        private async Task SaveUserProgress(string userId)
        {
            var user = new Users
            {
                userId = userId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = Phone,
                IsValid = "pending",
                IsAdmin = false
            };

            string userJson = JsonSerializer.Serialize(user);
            await SecureStorage.SetAsync("UserData", userJson);
            await SecureStorage.SetAsync("UserId", userId);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Password))
            {
                Shell.Current.DisplayAlert("Error", "Por favor, complete todos los campos", "OK");
                return false;
            }

            if (!IsValidEmail(Email) || !IsValidPassword(Password) || !IsValidPhoneNumber(Phone))
                return false;

            return true;
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