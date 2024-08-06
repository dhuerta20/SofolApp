using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Views
{
    public partial class ReferencesPage : ContentPage
    {
        private FirebaseConnection _firebaseConnection;
        private User _userData;

        public ReferencesPage()
        {
            InitializeComponent();
            _firebaseConnection = new FirebaseConnection();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            var userId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(userId))
            {
                _userData = await _firebaseConnection.ReadUserDataAsync(userId);
                await DisplayCurrentReferences();
            }
        }

        private async Task DisplayCurrentReferences()
        {
            ReferencesStackLayout.Children.Clear();

            var references = await _firebaseConnection.GetReferencesAsync(_userData.userId);
            foreach (var reference in references)
            {
                var referenceEntry = new Entry { Text = reference, IsReadOnly = true };
                ReferencesStackLayout.Children.Add(referenceEntry);
            }

            if (references.Count < 3)
            {
                var addReferenceEntry = new Entry { Placeholder = "Ingrese el correo de referencia" };
                var addButton = new Button
                {
                    Text = "Agregar Referencia",
                    BackgroundColor = Color.FromArgb("#5dd684"),
                    TextColor = Colors.White,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                addButton.Clicked += async (sender, args) => await AddReferenceAsync(addReferenceEntry.Text);
                ReferencesStackLayout.Children.Add(addReferenceEntry);
                ReferencesStackLayout.Children.Add(addButton);
            }
        }


        private async Task AddReferenceAsync(string referenceEmail)
        {
            if (string.IsNullOrEmpty(referenceEmail))
            {
                await DisplayAlert("Error", "Por favor, ingrese un correo de referencia.", "OK");
                return;
            }

            if (referenceEmail == _userData.Email)
            {
                await DisplayAlert("Error", "No puede referenciarse a sí mismo.", "OK");
                return;
            }

            var referenceExists = await _firebaseConnection.CheckIfUserExistsAsync(referenceEmail);
            if (!referenceExists)
            {
                await DisplayAlert("Error", "El correo de referencia no existe.", "OK");
                return;
            }

            var references = await _firebaseConnection.GetReferencesAsync(_userData.userId);
            if (references.Contains(referenceEmail))
            {
                await DisplayAlert("Error", "Esta referencia ya existe.", "OK");
                return;
            }

            await _firebaseConnection.AddReferenceAsync(_userData.userId, referenceEmail);
            await DisplayAlert("Éxito", "Referencia agregada exitosamente.", "OK");
            await DisplayCurrentReferences();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreditApp");
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//CreditApp");
            });

            return true; // Prevent default back button action
        }
    }
}
