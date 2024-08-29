using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using SofolApp.MVVM.Views;
using SofolApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SofolApp.MVVM.ViewModels
{
    public partial class ReferencesPageVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private Users _userData;

        [ObservableProperty]
        private ObservableCollection<string> _references;

        [ObservableProperty]
        private string _newReferenceEmail;

        [ObservableProperty]
        private bool _canAddReference;

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
                UpdateCanAddReference();
            }
        }

        private async Task DisplayCurrentReferencesAsync()
        {
            References.Clear();
            var references = await _firebaseConnection.GetReferencesAsync(_userData.userId);
            foreach (var reference in references)
            {
                References.Add(reference);
            }
        }

        private void UpdateCanAddReference()
        {
            CanAddReference = References.Count < 3;
        }

        [RelayCommand]
        public async Task AddReferenceAsync()
        {
            if (string.IsNullOrEmpty(NewReferenceEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, ingrese un correo de referencia.", "OK");
                return;
            }

            if (NewReferenceEmail == _userData.Email)
            {
                await Shell.Current.DisplayAlert("Error", "No puede referenciarse a sí mismo.", "OK");
                return;
            }

            var referenceExists = await _firebaseConnection.CheckIfUserExistsAsync(NewReferenceEmail);
            if (!referenceExists)
            {
                await Shell.Current.DisplayAlert("Error", "El correo de referencia no existe.", "OK");
                return;
            }

            if (References.Contains(NewReferenceEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Esta referencia ya existe.", "OK");
                return;
            }

            await _firebaseConnection.AddReferenceAsync(_userData.userId, NewReferenceEmail);
            await Shell.Current.DisplayAlert("Éxito", "Referencia agregada exitosamente.", "OK");
            await DisplayCurrentReferencesAsync();
            UpdateCanAddReference();
            NewReferenceEmail = string.Empty;
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync(nameof(CreditPage));
        }
    }
}