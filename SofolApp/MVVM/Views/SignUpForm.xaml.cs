using SofolApp.MVVM.ViewModels;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpForm : ContentPage
    {
        private FirebaseConnection firebaseConnection;

        public SignUpForm()
        {
            InitializeComponent();
            firebaseConnection = new FirebaseConnection();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void SignUpClicked(object sender, EventArgs e)
        {
            string firstName = firstNameEntry.Text;
            string lastName = lastNameEntry.Text;
            string email = emailEntry.Text;
            string phone = phoneEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Por favor, complete todos los campos", "OK");
                return;
            }

            // Validaci�n de email
            if (!IsValidEmail(email))
            {
                await DisplayAlert("Error", "Por favor, ingrese un correo v�lido", "OK");
                return;
            }

            // Validaci�n de contrase�a
            if (!IsValidPassword(password))
            {
                await DisplayAlert("Error", "La contrase�a debe tener al menos 8 caracteres, incluir may�sculas, min�sculas, n�meros y al menos un car�cter especial", "OK");
                return;
            }

            // Validaci�n de tel�fono
            if (!IsValidPhoneNumber(phone))
            {
                await DisplayAlert("Error", "Por favor, ingrese un n�mero de tel�fono v�lido de 10 d�gitos", "OK");
                return;
            }

            try
            {
                var userCredential = await firebaseConnection.CreateUserAsync(email, password, firstName, lastName, phone);

                if (userCredential != null && !string.IsNullOrEmpty(userCredential.User.Uid))
                {
                    // Guarda el email en SecureStorage
                    await SecureStorage.SetAsync("userEmail", email);

                    await Shell.Current.GoToAsync("//SignUpImg");
                    await DisplayAlert("�xito", "Usuario registrado correctamente", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo crear el usuario. Por favor, intente de nuevo.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detallado: {ex}");
                await DisplayAlert("Error", $"Error al registrar: {ex.Message}", "OK");
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

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//SignInForm");
            });
            return true;
        }
    }
}
