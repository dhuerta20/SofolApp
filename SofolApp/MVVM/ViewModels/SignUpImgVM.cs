using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using SofolApp.MVVM.Views;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpImgVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private readonly IAzureFaceService _azureFaceService;
        private readonly IRegistrationStateService _registrationStateService;
        private Users _currentUser;

        [ObservableProperty]
        private UploadFlags _uploadFlags;

        public SignUpImgVM(IFirebaseConnection firebaseConnection, IAzureFaceService azureFaceService, IRegistrationStateService registrationStateService)
        {
            _firebaseConnection = firebaseConnection;
            _azureFaceService = azureFaceService;
            _registrationStateService = registrationStateService;
            UploadFlags = new UploadFlags();
        }

        [RelayCommand]
        private async Task Initialize()
        {
            await LoadUserData();
            if (_currentUser == null || string.IsNullOrEmpty(_currentUser.userId))
            {
                await Shell.Current.DisplayAlert("Error", "User is not authenticated.", "OK");
                await Shell.Current.GoToAsync(nameof(SignInForm));
            }
            else
            {
                UpdateUploadFlags();
                await _registrationStateService.SaveRegistrationStateAsync("SignUpImg");
            }
        }


        private async Task LoadUserData()
        {
            string userJson = await SecureStorage.GetAsync("UserData");
            if (!string.IsNullOrEmpty(userJson))
            {
                _currentUser = JsonSerializer.Deserialize<Users>(userJson);
            }
            else
            {
                string userId = await SecureStorage.GetAsync("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    _currentUser = await _firebaseConnection.ReadUserDataAsync(userId);
                    if (_currentUser != null)
                    {
                        await SecureStorage.SetAsync("UserData", JsonSerializer.Serialize(_currentUser));
                    }
                }
            }
        }

        private void UpdateUploadFlags()
        {
            if (_currentUser?.Images != null)
            {
                UploadFlags.FaceUploaded = _currentUser.Images.ContainsKey("face");
                UploadFlags.IdUploaded = _currentUser.Images.ContainsKey("id");
                UploadFlags.AddressUploaded = _currentUser.Images.ContainsKey("address");
                UploadFlags.IncomeUploaded = _currentUser.Images.ContainsKey("income");
                UploadFlags.PayrollUploaded = _currentUser.Images.ContainsKey("payroll");
                UploadFlags.AccountStatusUploaded = _currentUser.Images.ContainsKey("accountStatus");
            }
        }

        [RelayCommand]
        private async Task UploadImage(string imageType)
        {
            try
            {
                var photoResult = await MediaPicker.CapturePhotoAsync();

                if (photoResult != null)
                {
                    using (var originalStream = await photoResult.OpenReadAsync())
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        await originalStream.CopyToAsync(memoryStream);

                        originalStream.Position = 0;
                        memoryStream.Position = 0;

                        if (imageType == "face")
                        {
                            if (!await VerifyFace(originalStream))
                                return;
                            memoryStream.Position = 0;
                        }

                        if (_currentUser.Images.ContainsKey(imageType))
                        {
                            await Shell.Current.DisplayAlert("Error", $"La imagen que intentas añadir ya existe ", "OK");
                            return;
                        }

                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_currentUser.userId, memoryStream, imageType);
                        _currentUser.Images[imageType] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_currentUser.userId, _currentUser);

                        UpdateUploadFlags();
                        await UpdateLocalUserData();

                        await Shell.Current.DisplayAlert("Éxito", "La imagen se agregó correctamente", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al añadir la imagen: {ex.Message}");
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
                        if (_currentUser.Images != null && _currentUser.Images.ContainsKey("accountStatus"))
                        {
                            await Shell.Current.DisplayAlert("Error", "Este archivo ya se agregó correctamente.", "OK");
                            return;
                        }

                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_currentUser.userId, stream, "accountStatus");

                        if (_currentUser.Images == null)
                        {
                            _currentUser.Images = new Dictionary<string, string>();
                        }
                        _currentUser.Images["accountStatus"] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_currentUser.userId, _currentUser);

                        UpdateUploadFlags();
                        await UpdateLocalUserData();

                        await Shell.Current.DisplayAlert("Éxito", "El estado de cuenta se subió correctamente.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al tratar de agregar el PDF: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"No se logró agregar el PDF: {ex.Message}", "OK");
            }
        }

        private async Task<bool> VerifyFace(Stream imageStream)
        {
            Console.WriteLine("Iniciando verificación de rostro con Azure Face API");
            bool isFace = await _azureFaceService.VerifyFaceAsync(imageStream);
            Console.WriteLine($"Resultado de la verificación de rostro: {isFace}");

            if (!isFace)
            {
                await Shell.Current.DisplayAlert("Error", "No se detectó un rostro humano en la imagen. Por favor, intenta de nuevo.", "OK");
                return false;
            }
            return true;
        }

        private async Task UpdateLocalUserData()
        {
            string userJson = JsonSerializer.Serialize(_currentUser);
            await SecureStorage.SetAsync("UserData", userJson);
        }

        [RelayCommand]
        private async Task NavigateToReferences()
        {
            if (AllUploadsCompleted())
            {
                await _registrationStateService.SaveRegistrationStateAsync("SignUpReferences");
                await Shell.Current.GoToAsync(nameof(SignUpReferences));
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, sube todas las imágenes antes de continuar.", "OK");
            }
        }

        [RelayCommand]
        private async Task EndRegister()
        {
            if (AllUploadsCompleted())
            {
                await _registrationStateService.ClearRegistrationStateAsync();
                await Shell.Current.GoToAsync(nameof(CreditPage));
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Por favor, sube todas las imágenes antes de finalizar.", "OK");
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
        private bool faceUploaded;

        [ObservableProperty]
        private bool idUploaded;

        [ObservableProperty]
        private bool addressUploaded;

        [ObservableProperty]
        private bool incomeUploaded;

        [ObservableProperty]
        private bool payrollUploaded;

        [ObservableProperty]
        private bool accountStatusUploaded;
    }
}