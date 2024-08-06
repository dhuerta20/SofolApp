using SofolApp.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace SofolApp.MVVM.Views
{
    public partial class CreditPage : ContentPage
    {
        private FirebaseConnection _firebaseConnection;


        public CreditPage()
        {
            InitializeComponent();
            _firebaseConnection = new FirebaseConnection();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserData();
            await CheckSession();

        }

        private async Task CheckSession()
        {
            bool isSessionValid = await SessionManager.CheckSessionCount();
            if (!isSessionValid)
            {
                await SignOut();
            }
            else
            {
                await LoadUserData();
            }
        }

        private async Task SignOut()
        {
            await _firebaseConnection.SignOutAsync();
            await Shell.Current.GoToAsync("//SignInForm");
        }

        private async Task LoadUserData()
        {
            try
            {
                var userId = await SecureStorage.GetAsync("userId");
                var user = await _firebaseConnection.ReadUserDataAsync(userId);

                if (user.Images != null)
                {
                    foreach (var imageEntry in user.Images)
                    {
                        if (imageEntry.Key.Contains("face"))
                        {
                            ProfileImage.Source = imageEntry.Value;
                            break;  // Asumimos que solo hay una imagen de perfil
                        }
                    }
                }

                UserNameLabel.Text = $"{user.FirstName}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            try
            {
                // Cerrar sesi�n
                await _firebaseConnection.SignOutAsync();

                // Redirigir a la p�gina de inicio de sesi�n
                await Shell.Current.GoToAsync("//SignInForm");
            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar mensaje de error si es necesario
                await DisplayAlert("Error", $"No se pudo cerrar la sesi�n: {ex.Message}", "OK");
            }
        }

        private async void OnIdentityConfirmClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//IdConfirm");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error de Navegaci�n", $"No se pudo navegar a PersonalData: {ex.Message}", "OK");
            }
        }

        private async void OnPersonalDataClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//PersonalData");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error de Navegaci�n", $"No se pudo navegar a PersonalData: {ex.Message}", "OK");
            }
        }

        private async void OnStatusClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Status");
        }

        private async void OnWhatsAppTapped(object sender, EventArgs e)
        {
            var phoneNumber = "+528130520634"; // Reemplaza con el n�mero de WhatsApp real
            var message = "Hola, me gustar�a obtener m�s informaci�n.";
            var url = $"https://wa.me/{phoneNumber}?text={Uri.EscapeDataString(message)}";
            await Launcher.OpenAsync(new Uri(url));
        }

        private async void OnPhoneTapped(object sender, EventArgs e)
        {
            var phoneNumber = "+528130520634"; // Reemplaza con el n�mero de tel�fono real
            try
            {
                PhoneDialer.Open(phoneNumber);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo realizar la llamada: " + ex.Message, "OK");
            }
        }

        private async void OnEmailTapped(object sender, EventArgs e)
        {
            var email = "appdecredito@proxcredit.com"; // Reemplaza con la direcci�n de correo real
            var subject = "Consulta";
            var body = "Hola, me gustar�a obtener m�s informaci�n.";
            var uri = $"mailto:{email}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            await Launcher.OpenAsync(new Uri(uri));
        }
    }
}
