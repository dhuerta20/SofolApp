using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using Microsoft.Maui.Storage;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpImgVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private string _userId;

        [ObservableProperty]
        private UploadFlags _uploadFlags;

        public SignUpImgVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
            UploadFlags = new UploadFlags();
        }

        [RelayCommand]
        private async Task Initialize()
        {
            _userId = await SecureStorage.GetAsync("userId");
            if (string.IsNullOrEmpty(_userId))
            {
                await Shell.Current.DisplayAlert("Error", "User is not authenticated.", "OK");
                await Shell.Current.GoToAsync("//Login");
            }
        }

        [RelayCommand]
        private async Task UploadImage(string imageType)
        {
            try
            {
                var filePickerResult = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = $"Select {imageType} photo",
                    FileTypes = FilePickerFileType.Images
                });

                if (filePickerResult != null)
                {
                    using (var stream = await filePickerResult.OpenReadAsync())
                    {
                        var user = await _firebaseConnection.ReadUserDataAsync(_userId);
                        if (user.Images != null && user.Images.ContainsKey(imageType))
                        {
                            await Shell.Current.DisplayAlert("Error", $"La imagen que intentas añadir ya existe ", "OK");
                            return;
                        }

                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_userId, stream, imageType);
                        UpdateUploadFlags(imageType, true);

                        if (user.Images == null)
                        {
                            user.Images = new Dictionary<string, string>();
                        }
                        user.Images[imageType] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_userId, user);

                        await Shell.Current.DisplayAlert("Éxito", "La imagen se agrego correctamente", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al añadir la imagen : {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"No se pudo agregar la foto: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task UploadAccountStatus()
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { ".pdf" } },
                        { DevicePlatform.macOS, new[] { "pdf" } }
                    }),
                    PickerTitle = "Select your account statement PDF"
                });

                if (fileResult != null)
                {
                    using (var stream = await fileResult.OpenReadAsync())
                    {
                        var user = await _firebaseConnection.ReadUserDataAsync(_userId);
                        if (user.Images != null && user.Images.ContainsKey("accountStatus"))
                        {
                            await Shell.Current.DisplayAlert("Error", "Este archivo ya se agrego correctamente.", "OK");
                            return;
                        }

                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_userId, stream, "accountStatus");
                        UpdateUploadFlags("accountStatus", true);

                        if (user.Images == null)
                        {
                            user.Images = new Dictionary<string, string>();
                        }
                        user.Images["accountStatus"] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_userId, user);

                        await Shell.Current.DisplayAlert("Éxito", "El estado de cuenta se subio correctamente.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al tratar de agregar el PDF: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"No se logro agregar el PDF: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToReferences()
        {
            try
            {
                bool uploadsCompleted = AllUploadsCompleted();

                if (uploadsCompleted)
                {
                    await Shell.Current.GoToAsync("//SignUpReferences");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Porfavor agrega todas las imagenes anteriores antes de subir el archivo.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error de navegación", $"Fallo al tratar de navegar a la página de referencias: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EndRegister()
        {
            if (AllUploadsCompleted())
            {
                await Shell.Current.GoToAsync("//CreditApp");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Porfavor agrega todas las imágenes antes de finalizar.", "OK");
            }
        }

        private void UpdateUploadFlags(string imageType, bool isUploaded)
        {
            switch (imageType)
            {
                case "face": UploadFlags.FaceUploaded = isUploaded; break;
                case "id": UploadFlags.IdUploaded = isUploaded; break;
                case "address": UploadFlags.AddressUploaded = isUploaded; break;
                case "income": UploadFlags.IncomeUploaded = isUploaded; break;
                case "payroll": UploadFlags.PayrollUploaded = isUploaded; break;
                case "accountStatus": UploadFlags.AccountStatusUploaded = isUploaded; break;
            }
        }

        private bool AllUploadsCompleted()
        {
            return UploadFlags.FaceUploaded &&
                   UploadFlags.IdUploaded &&
                   UploadFlags.AddressUploaded &&
                   UploadFlags.IncomeUploaded &&
                   UploadFlags.PayrollUploaded &&
                   UploadFlags.AccountStatusUploaded;
        }
    }

    public partial class UploadFlags : ObservableObject
    {
        [ObservableProperty]
        private bool _faceUploaded;

        [ObservableProperty]
        private bool _idUploaded;

        [ObservableProperty]
        private bool _addressUploaded;

        [ObservableProperty]
        private bool _incomeUploaded;

        [ObservableProperty]
        private bool _payrollUploaded;

        [ObservableProperty]
        private bool _accountStatusUploaded;
    }
}