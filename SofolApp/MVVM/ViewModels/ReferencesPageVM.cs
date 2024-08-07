using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using SofolApp.Services;

namespace SofolApp.MVVM.ViewModels
{
    public partial class ReferencesPageVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private Users _userData;

        [ObservableProperty]
        private ObservableCollection<string> _references;

        public ReferencesPageVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
            References = new ObservableCollection<string>();
        }

        [RelayCommand]
        public async Task LoadUserDataAsync()
        {
            var userId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(userId))
            {
                _userData = await _firebaseConnection.ReadUserDataAsync(userId);
                await DisplayCurrentReferencesAsync();
            }
        }

        [RelayCommand]
        private async Task DisplayCurrentReferencesAsync()
        {
            References.Clear();
            var references = await _firebaseConnection.GetReferencesAsync(_userData.userId);
            foreach (var reference in references)
            {
                References.Add(reference);
            }
        }

        [RelayCommand]
        public async Task AddReferenceAsync(string referenceEmail)
        {
            if (string.IsNullOrEmpty(referenceEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, ingrese un correo de referencia.", "OK");
                return;
            }

            if (referenceEmail == _userData.Email)
            {
                await Shell.Current.DisplayAlert("Error", "No puede referenciarse a sí mismo.", "OK");
                return;
            }

            var referenceExists = await _firebaseConnection.CheckIfUserExistsAsync(referenceEmail);
            if (!referenceExists)
            {
                await Shell.Current.DisplayAlert("Error", "El correo de referencia no existe.", "OK");
                return;
            }

            if (References.Contains(referenceEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Esta referencia ya existe.", "OK");
                return;
            }

            await _firebaseConnection.AddReferenceAsync(_userData.userId, referenceEmail);
            await Shell.Current.DisplayAlert("Éxito", "Referencia agregada exitosamente.", "OK");
            await DisplayCurrentReferencesAsync();
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("//CreditApp");
        }
    }
}

