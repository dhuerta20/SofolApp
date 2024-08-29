using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using SofolApp.Services;
using System;
using System.Threading.Tasks;
using Sentry;
using System.Windows.Input;
using SofolApp.MVVM.Views;

namespace SofolApp.MVVM.ViewModels
{
    public partial class CreditPageVM : ObservableObject
    {

        #region Commands


        public ICommand LogOutCommand => new Command(async () =>
        {
            await OnSignOut();
        });


        public ICommand IdentityConfirmationCommand => new Command(async () =>
        {
            await OnIdentityConfirm();
        });

        public ICommand PersonalProfileDataCommand => new Command(async () =>
        {
            await OnPersonalData();
        });


        public ICommand StatusRequestCommand => new Command(async () =>
        {
            await OnStatus();
        });

       /* public ICommand IWhatsAppCommand => new Command(async () =>
        {
            await OnWhatsApp();
        });


        public ICommand PhoneCallCommand => new Command(async () =>
        {
            await OnPhone();
        });*/

        public ICommand SendEmailCommand => new Command(async () =>
        {
            await OnEmail();
        });

        #endregion

        private readonly IFirebaseConnection _firebaseConnection;

        [ObservableProperty]
        private string _profileImage;

        [ObservableProperty]
        private string _userName;

        public CreditPageVM(IFirebaseConnection firebaseConnection)
        {
            try
            {
                _firebaseConnection = firebaseConnection;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        [RelayCommand]
        public async Task OnSignOut()
        {
            try
            {
                await _firebaseConnection.SignOutAsync();
                await Shell.Current.GoToAsync($"//{nameof(SignInForm)}");

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo cerrar la sesión", "OK");
            }
        }

        [RelayCommand]
        public async Task OnIdentityConfirm()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(IdConfirmPage));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo navegar a Confirmación de Identidad", "OK");
            }
        }

        [RelayCommand]
        public async Task OnPersonalData()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(PersonalDataPage));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo navegar a Datos Personales", "OK");
            }
        }

        [RelayCommand]
        public async Task OnStatus()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(ReferencesPage));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo navegar a Estatus", "OK");
            }
        }

/*        [RelayCommand]
        public async Task OnWhatsApp()
        {
            try
            {
                var phoneNumber = "";
                var message = "Hola, me gustaría obtener más información.";
                var url = $"https://wa.me/{phoneNumber}?text={Uri.EscapeDataString(message)}";
                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir WhatsApp", "OK");
            }
        }

        [RelayCommand]
        public async Task OnPhone()
        {
            try
            {
                var phoneNumber = "";
                PhoneDialer.Open(phoneNumber);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo realizar la llamada", "OK");
            }
        }
*/
        [RelayCommand]
        public async Task OnEmail()
        {
            try
            {
                var email = "appdecredito@proxcredit.com";
                var subject = "Consulta";
                var body = "Hola, me gustaría obtener más información.";
                var uri = $"mailto:{email}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
                await Launcher.OpenAsync(new Uri(uri));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir el correo", "OK");
            }
        }

        public async Task LoadUserData()
        {
            try
            {
                Console.WriteLine("LoadUserData started");
                var userId = await SecureStorage.GetAsync("userId");
                Console.WriteLine($"UserId: {userId}");
                var user = await _firebaseConnection.ReadUserDataAsync(userId);
                Console.WriteLine($"User data loaded: {user != null}");

                if (user?.Images != null)
                {
                    foreach (var imageEntry in user.Images)
                    {
                        if (imageEntry.Key.Contains("face"))
                        {
                            ProfileImage = imageEntry.Value;
                            Console.WriteLine($"ProfileImage set: {ProfileImage}");
                            break;
                        }
                    }
                }

                UserName = user?.FirstName ?? "No se ha agregado una referencia";
                Console.WriteLine($"UserName set: {UserName}");
                OnPropertyChanged(nameof(ProfileImage));
                OnPropertyChanged(nameof(UserName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadUserData: {ex}");
                SentrySdk.CaptureException(ex);
            }
        }
    }
}