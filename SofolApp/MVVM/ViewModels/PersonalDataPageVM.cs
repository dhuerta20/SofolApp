using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Type;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using SofolApp.MVVM.Models;
using SofolApp.MVVM.Views;
using SofolApp.Services;

namespace SofolApp.MVVM.ViewModels
{
    public partial class PersonalDataPageVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
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

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _phoneNumber;

        public PersonalDataPageVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
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
                // Validar que los campos no sean nulos o estén vacíos
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                    return;
                }

                // Validar que el número de teléfono tenga 10 dígitos
                if (PhoneNumber.Length != 10 || !PhoneNumber.All(char.IsDigit))
                {
                    await Shell.Current.DisplayAlert("Error", "El número de teléfono debe tener 10 dígitos.", "OK");
                    return;
                }

                CurrentUser.FirstName = FirstName;
                CurrentUser.LastName = LastName;
                CurrentUser.PhoneNumber = PhoneNumber;
                // No actualizamos el Email ya que no debe cambiar

                await _firebaseConnection.UpdateUserDataAsync(_userId, CurrentUser);
                await Shell.Current.DisplayAlert("Éxito", "Los datos del usuario se han actualizado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al guardar los cambios: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync(nameof(CreditPage));
        }

        private async Task LoadUserData()
        {
            try
            {
                CurrentUser = await _firebaseConnection.ReadUserDataAsync(_userId);
                FirstName = CurrentUser.FirstName;
                LastName = CurrentUser.LastName;
                Email = CurrentUser.Email;
                PhoneNumber = CurrentUser.PhoneNumber;
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