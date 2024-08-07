using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using SofolApp.MVVM.Views;
using SofolApp.Services;
using System;
using System.Threading.Tasks;

namespace SofolApp.MVVM.ViewModels
{
    public partial class CreditPageVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private readonly SessionManager _sessionManager;

        [ObservableProperty]
        private string _profileImage;

        [ObservableProperty]
        private string _userName;

        public CreditPageVM(IFirebaseConnection firebaseConnection, SessionManager sessionManager)
        {
            _firebaseConnection = firebaseConnection;
            _sessionManager = sessionManager;
        }

        [RelayCommand]
        public async Task OnSignOut()
        {
            try
            {
                await _firebaseConnection.SignOutAsync();
                await Shell.Current.GoToAsync("//SignInForm");
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("No se pudo cerrar la sesión", ex.Message);
            }
        }

        [RelayCommand]
        public async Task OnIdentityConfirm()
        {
            try
            {
                await Shell.Current.GoToAsync("//IdConfirm");
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Error de Navegación", $"No se pudo navegar a Confirmación de Identidad: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OnPersonalData()
        {
            try
            {
                await Shell.Current.GoToAsync("//PersonalData");
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Error de Navegación", $"No se pudo navegar a Datos Personales: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OnStatus()
        {
            try
            {
                await Shell.Current.GoToAsync("//Status");
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Error de Navegación", $"No se pudo navegar a Estatus: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OnWhatsApp()
        {
            var phoneNumber = "+528130520634"; // Reemplaza con el número de WhatsApp real
            var message = "Hola, me gustaría obtener más información.";
            var url = $"https://wa.me/{phoneNumber}?text={Uri.EscapeDataString(message)}";
            await Launcher.OpenAsync(new Uri(url));
        }

        [RelayCommand]
        public async Task OnPhone()
        {
            var phoneNumber = "+528130520634"; // Reemplaza con el número de teléfono real
            try
            {
                PhoneDialer.Open(phoneNumber);
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Error", $"No se pudo realizar la llamada: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OnEmail()
        {
            var email = "appdecredito@proxcredit.com"; // Reemplaza con la dirección de correo real
            var subject = "Consulta";
            var body = "Hola, me gustaría obtener más información.";
            var uri = $"mailto:{email}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            await Launcher.OpenAsync(new Uri(uri));
        }

        public async Task LoadUserData()
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
                            ProfileImage = imageEntry.Value;
                            break; // Asumimos que solo hay una imagen de perfil
                        }
                    }
                }

                UserName = user.FirstName;
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Error", $"No se pudieron cargar los datos del usuario: {ex.Message}");
            }
        }

        private async Task ShowErrorAlert(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }
    }
}