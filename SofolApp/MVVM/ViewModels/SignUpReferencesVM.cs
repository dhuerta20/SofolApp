using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using SofolApp.MVVM.Views;
using SofolApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpReferencesVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private readonly IRegistrationStateService _registrationStateService;
        private Users _userData;

        [ObservableProperty]
        private ObservableCollection<string> _references;

        [ObservableProperty]
        private string _newReferenceEmail;

        [ObservableProperty]
        private bool _canAddReference;

        [ObservableProperty]
        private bool _canNavigateAway;

        public SignUpReferencesVM(IFirebaseConnection firebaseConnection, IRegistrationStateService registrationStateService)
        {
            _firebaseConnection = firebaseConnection;
            _registrationStateService = registrationStateService;
            References = new ObservableCollection<string>();
            CanNavigateAway = false;
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            var userId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(userId))
            {
                _userData = await _firebaseConnection.ReadUserDataAsync(userId);
                await DisplayCurrentReferencesAsync();
                UpdateCanAddReference();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró el id del usuario. Ingrese de nuevo, por favor.", "OK");
                await Shell.Current.GoToAsync(nameof(SignUpForm));
            }
        }

        private async Task DisplayCurrentReferencesAsync()
        {
            References.Clear();
            if (_userData != null)
            {
                if (!string.IsNullOrEmpty(_userData.FirstReference))
                    References.Add(_userData.FirstReference);
                if (!string.IsNullOrEmpty(_userData.SecondReference))
                    References.Add(_userData.SecondReference);
                if (!string.IsNullOrEmpty(_userData.ThirdReference))
                    References.Add(_userData.ThirdReference);
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

            References.Add(NewReferenceEmail);
            UpdateUserReferences();

            try
            {
                await _firebaseConnection.UpdateUserDataAsync(_userData.userId, _userData);
                await Shell.Current.DisplayAlert("Éxito", "La referencia se agregó y guardó correctamente.", "OK");
                CanNavigateAway = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddReferenceAsync: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al guardar la referencia. Por favor, intente nuevamente.", "OK");
                References.Remove(NewReferenceEmail);
                UpdateUserReferences();
                CanNavigateAway = false;
            }

            UpdateCanAddReference();
            NewReferenceEmail = string.Empty;
        }

        private void UpdateUserReferences()
        {
            _userData.FirstReference = References.Count > 0 ? References[0] : null;
            _userData.SecondReference = References.Count > 1 ? References[1] : null;
            _userData.ThirdReference = References.Count > 2 ? References[2] : null;
        }

        [RelayCommand]
        public async Task ReturnToRegisterAsync()
        {
            if (CanNavigateAway)
            {
                await _registrationStateService.ClearRegistrationStateAsync();
                await Shell.Current.GoToAsync(nameof(SignUpImg));
            }
            else
            {
                bool answer = await Shell.Current.DisplayAlert("Advertencia", "No has guardado las referencias. ¿Estás seguro de que quieres volver sin guardar?", "Sí", "No");
                if (answer)
                {
                    await Shell.Current.GoToAsync(nameof(SignUpImg));
                }
            }
        }
    }
}