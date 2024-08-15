using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using SofolApp.MVVM.Models;
using SofolApp.MVVM.Services;
using SofolApp.Services;

namespace SofolApp.MVVM.ViewModels
{
    public partial class PersonalDataPageVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private readonly IMediaService _mediaService;
        private string _userId;

        [ObservableProperty]
        private Users _currentUser;

        [ObservableProperty]
        private ImageSource _facePhotoSource;

        [ObservableProperty]
        private ImageSource _idPhotoSource;

        [ObservableProperty]
        private ImageSource _addressPhotoSource;

        [ObservableProperty]
        private ImageSource _incomePhotoSource;

        [ObservableProperty]
        private ImageSource _payrollPhotoSource;

        [ObservableProperty]
        private ImageSource _accountStatusPhotoSource;

        public PersonalDataPageVM(IFirebaseConnection firebaseConnection, IMediaService mediaService)
        {
            _firebaseConnection = firebaseConnection;
            _mediaService = mediaService;
        }

        [RelayCommand]
        private async Task InitializeAsync()
        {
            _userId = await SecureStorage.GetAsync("userId");
            await LoadUserData();
        }

        [RelayCommand]
        private async Task UpdatePhoto(string imageType)
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    var url = await UploadImage(photo, imageType);
                    if (url != null)
                    {
                        if (CurrentUser.Images == null)
                            CurrentUser.Images = new Dictionary<string, string>();

                        // Actualizar la URL de la imagen existente
                        CurrentUser.Images[imageType] = url;

                        await LoadUserImages();
                        await _firebaseConnection.UpdateUserDataAsync(_userId, CurrentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update photo: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            try
            {
                await _firebaseConnection.UpdateUserDataAsync(_userId, CurrentUser);
                await Shell.Current.DisplayAlert("Éxito", "Los datos del usuario se han actualizado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save changes: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("//CreditApp");
        }

        private async Task LoadUserData()
        {
            try
            {
                CurrentUser = await _firebaseConnection.ReadUserDataAsync(_userId);
                await LoadUserImages();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
            }
        }

        private async Task LoadUserImages()
        {
            if (CurrentUser.Images != null)
            {
                FacePhotoSource = await GetImageSource("face");
                IdPhotoSource = await GetImageSource("id");
                AddressPhotoSource = await GetImageSource("address");
                IncomePhotoSource = await GetImageSource("income");
                PayrollPhotoSource = await GetImageSource("payroll");
                AccountStatusPhotoSource = await GetImageSource("accountStatus");
            }
        }

        private async Task<ImageSource> GetImageSource(string imageType)
        {
            if (CurrentUser.Images.TryGetValue(imageType, out var url) && !string.IsNullOrEmpty(url))
            {
                return ImageSource.FromUri(new Uri(url));
            }
            return null;
        }

        private async Task<string> UploadImage(FileResult photo, string imageType)
        {
            try
            {
                if (photo != null)
                {
                    using (var stream = await photo.OpenReadAsync())
                    {
                        return await _firebaseConnection.UploadImageAsync(_userId, stream, imageType);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to upload image: {ex.Message}", "OK");
            }
            return null;
        }
    }
}