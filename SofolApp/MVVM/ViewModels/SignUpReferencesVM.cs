using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Firebase.Database.Query;
using Firebase.Auth;
using SofolApp.MVVM.Models;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpReferencesVM : ObservableObject
    {
        private readonly FirebaseConnection _firebaseConnection;
        private string _userId;

        [ObservableProperty]
        private ObservableCollection<string> referenceEmails;

        public SignUpReferencesVM()
        {
            _firebaseConnection = new FirebaseConnection();
            ReferenceEmails = new ObservableCollection<string>();

            InitializeUserId();
        }

        private async Task InitializeUserId()
        {
            _userId = await SecureStorage.GetAsync("userId");
            if (string.IsNullOrEmpty(_userId))
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró el id del usuario. Ingrese de nuevo, por favor.", "OK");
                await Shell.Current.GoToAsync("//SignUp");
            }
        }

        [RelayCommand]
        private void AddReference()
        {
            if (ReferenceEmails.Count < 3)
            {
                ReferenceEmails.Add("");
            }
            else
            {
                Shell.Current.DisplayAlert("Límite alcanzado", "Solo se permiten 3 referencias", "OK");
            }
        }

        [RelayCommand]
        private async Task ValidateReference(int index)
        {
            string email = ReferenceEmails[index].Trim();
            if (string.IsNullOrWhiteSpace(email)) return;

            if (await IsCurrentUserReference(email))
            {
                await Shell.Current.DisplayAlert("Error", "No puedes referenciarte a ti mismo", "OK");
                ReferenceEmails[index] = "";
                return;
            }

            if (!await IsValidUserReference(email))
            {
                await Shell.Current.DisplayAlert("Referencia no válida", $"El usuario con el email: {email} no existe en la app o no está aprobado", "OK");
                ReferenceEmails[index] = "";
                return;
            }

            if (IsDuplicateReference(email))
            {
                await Shell.Current.DisplayAlert("Referencia duplicada", $"El usuario con el email: {email} ya ha sido agregado como referencia", "OK");
                ReferenceEmails[index] = "";
            }
        }

        private bool IsDuplicateReference(string email)
        {
            return ReferenceEmails.Count(e => e.Equals(email, StringComparison.OrdinalIgnoreCase)) > 1;
        }

        private async Task<bool> IsCurrentUserReference(string email)
        {
            string currentUserEmail = await SecureStorage.GetAsync("userEmail");
            return email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> IsValidUserReference(string email)
        {
            bool userExists = await _firebaseConnection.CheckIfUserExistsAsync(email);
            if (!userExists)
            {
                return false;
            }

            var client = FirebaseConnection.GetDatabaseClient();
            var user = (await client.Child("users")
                .OrderBy("Email")
                .EqualTo(email)
                .OnceAsync<Users>())
                .FirstOrDefault();

            return user?.Object?.IsValid == "approved";
        }

        [RelayCommand]
        private async Task SaveReferences()
        {
            try
            {
                var currentUser = await _firebaseConnection.ReadUserDataAsync(_userId);

                for (int i = 0; i < ReferenceEmails.Count; i++)
                {
                    string email = ReferenceEmails[i].Trim();
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        switch (i)
                        {
                            case 0:
                                currentUser.FirstReference = email;
                                break;
                            case 1:
                                currentUser.SecondReference = email;
                                break;
                            case 2:
                                currentUser.ThirdReference = email;
                                break;
                        }
                    }
                }

                await _firebaseConnection.UpdateUserDataAsync(_userId, currentUser);

                await Shell.Current.DisplayAlert("Éxito", "Las referencias se guardaron correctamente!", "OK");
                await Shell.Current.GoToAsync("//SignUpImg");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo guardar las referencias: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ReturnToRegister()
        {
            await Shell.Current.GoToAsync("//SignUpImg");
        }
    }
}