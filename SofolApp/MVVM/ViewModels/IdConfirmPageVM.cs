using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SofolApp.MVVM.ViewModels
{
    public partial class IdConfirmPageVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;

        public IdConfirmPageVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
            LoadUserStatusCommand = new AsyncRelayCommand(LoadUserStatusAsync);
        }

        [ObservableProperty]
        private string profileImage;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string statusImage;

        [ObservableProperty]
        private string statusLabel;

        [ObservableProperty]
        private Color statusColor;

        [ObservableProperty]
        private string adminNotes;

        public IAsyncRelayCommand LoadUserStatusCommand { get; }

        private async Task LoadUserStatusAsync()
        {
            try
            {
                string userId = await SecureStorage.GetAsync("userId");
                var user = await _firebaseConnection.ReadUserDataAsync(userId);

                if (user.Images != null)
                {
                    foreach (var imageEntry in user.Images)
                    {
                        if (imageEntry.Key.Contains("face"))
                        {
                            ProfileImage = imageEntry.Value;
                        }
                    }
                }

                UserName = $"{user.FirstName}";

                switch (user.IsValid.ToLower())
                {
                    case "approved":
                        StatusImage = "aproved.png";
                        StatusLabel = "Pre aprobado";
                        StatusColor = Colors.Green;
                        break;
                    case "disapproved":
                        StatusImage = "disaproved.png";
                        StatusLabel = "No aprobado";
                        StatusColor = Colors.Red;
                        break;
                    default:
                        StatusImage = "pending.png";
                        StatusLabel = "Pendiente";
                        StatusColor = Colors.Orange;
                        break;
                }

                AdminNotes = user.AdminNotes;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar los datos del usuario: {ex.Message}", "OK");
            }
        }
    }
}
