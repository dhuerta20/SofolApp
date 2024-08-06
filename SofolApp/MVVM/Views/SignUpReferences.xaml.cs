using SofolApp.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Firebase.Database.Query;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpReferences : ContentPage
    {
        private FirebaseConnection _firebaseConnection;
        private string userId;
        private List<Entry> referenceEntries = new List<Entry>();

        public SignUpReferences()
        {
            InitializeComponent();
            _firebaseConnection = new FirebaseConnection();
            InitializeUserId();

            AddReferenceEntry();
            AddReferenceEntry();
            AddReferenceEntry();
        }

        private async Task InitializeUserId()
        {
            userId = await SecureStorage.GetAsync("userId");
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "No se encontró el id del usuario. Ingrese de nuevo, por favor.", "OK");
                await Shell.Current.GoToAsync("//SignUp");
            }
        }

        private void OnAddReferenceClicked(object sender, EventArgs e)
        {
            AddReferenceEntry();
        }

        private void AddReferenceEntry(string initialValue = "")
        {
            if (referenceEntries.Count >= 3)
            {
                DisplayAlert("Límite alcanzado", "Solo se permiten 3 referencias", "OK");
                return;
            }

            int referenceNumber = referenceEntries.Count + 1;
            string placeholder = $"Referencia {referenceNumber} (Email)";

            var entry = new Entry
            {
                Placeholder = placeholder,
                Text = initialValue,
                Keyboard = Keyboard.Email
            };

            entry.Completed += async (sender, e) =>
            {
                if (!string.IsNullOrWhiteSpace(entry.Text))
                {
                    string email = entry.Text.Trim();
                    if (!await IsValidUserReference(email))
                    {
                        await DisplayAlert("Referencia no válida", $"El usuario con el email: {email} no existe en la app o no está aprobado", "OK");
                        entry.Text = "";
                    }
                    else if (IsDuplicateReference(email))
                    {
                        await DisplayAlert("Referencia duplicada", $"El usuario con el email: {email} ya ha sido agregado como referencia", "OK");
                        entry.Text = "";
                    }
                    else if (await IsCurrentUserReference(email))
                    {
                        await DisplayAlert("Error", "No puedes referenciarte a ti mismo", "OK");
                        entry.Text = "";
                    }
                }
            };

            referenceEntries.Add(entry);
            ReferencesContainer.Children.Add(entry);
            UpdateAddReferenceButtonState();
        }

        private bool IsDuplicateReference(string email)
        {
            return referenceEntries.Any(entry => entry.Text.Equals(email, StringComparison.OrdinalIgnoreCase));
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

            // Check if the user is validated
            var client = FirebaseConnection.GetDatabaseClient();
            var user = (await client.Child("users")
                .OrderBy("Email")
                .EqualTo(email)
                .OnceAsync<User>())
                .FirstOrDefault();

            return user?.Object?.IsValid == "approved";
        }

        private void UpdateAddReferenceButtonState()
        {
            AddReferenceButton.IsEnabled = referenceEntries.Count < 3;
        }

        private async void OnSaveReferencesClicked(object sender, EventArgs e)
        {
            try
            {
                var currentUser = await _firebaseConnection.ReadUserDataAsync(userId);

                for (int i = 0; i < referenceEntries.Count; i++)
                {
                    string email = referenceEntries[i].Text.Trim();
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        if (await IsCurrentUserReference(email))
                        {
                            await DisplayAlert("Error", "No puedes referenciarte a ti mismo", "OK");
                            return;
                        }

                        if (!await IsValidUserReference(email))
                        {
                            await DisplayAlert("Error", $"El correo de referencia {email} no es válido o no está aprobado", "OK");
                            return;
                        }

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

                await _firebaseConnection.UpdateUserDataAsync(userId, currentUser);

                await DisplayAlert("Éxito", "Las referencias se guardaron correctamente!", "OK");
                await Shell.Current.GoToAsync("//SignUpImg");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar las referencias: {ex.Message}", "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//SignUpImg");
            });

            return true;
        }

        private async void ReturnToRegister(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpImg");
        }
    }
}
