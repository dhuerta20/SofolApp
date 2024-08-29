using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using System.Threading.Tasks;

namespace SofolApp.MVVM.ViewModels
{
    public partial class ForgotPassVM : ObservableObject
    {
        private IFirebaseConnection _firebaseConnection;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        public ForgotPassVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
        }

        [RelayCommand]
        public async Task SendCodeToEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor, ingrese su correo electrónico", "OK");
                return;
            }
            IsBusy = true;
            try
            {
                await _firebaseConnection.SendPasswordResetEmailAsync(Email);
                await Application.Current.MainPage.DisplayAlert("Éxito", "Se ha enviado un correo de restablecimiento. Por favor, revise su bandeja de entrada y siga las instrucciones para restablecer su contraseña.", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo enviar el correo: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}