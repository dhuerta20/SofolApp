using SofolApp.MVVM.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;

namespace SofolApp.MVVM.Views
{
    public partial class PersonalDataPage : ContentPage
    {
        private readonly FirebaseConnection _firebaseConnection;
        private string userId;
        private User currentUser;

        public PersonalDataPage()
        {
            InitializeComponent();
            _firebaseConnection = new FirebaseConnection();

            // Configurar el comportamiento del botón de retroceso
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () => await GoBack())
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            userId = await SecureStorage.GetAsync("userId");
            await LoadUserData();
            await LoadUserImages();
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        protected override bool OnBackButtonPressed()
        {
            GoBack();
            return true;
        }

        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("//CreditApp");
        }

        private async Task LoadUserData()
        {
            try
            {
                currentUser = await _firebaseConnection.ReadUserDataAsync(userId);

                firstNameEntry.Text = currentUser.FirstName;
                lastNameEntry.Text = currentUser.LastName;
                emailEntry.Text = currentUser.Email;
                phoneNumberEntry.Text = currentUser.PhoneNumber;

                await LoadUserImages();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
            }
        }

        private async Task LoadUserImages()
        {
            try
            {
                var user = await _firebaseConnection.ReadUserDataAsync(userId);
                if (user.Images != null)
                {
                    if (user.Images.TryGetValue("face", out var faceUrl) && !string.IsNullOrEmpty(faceUrl))
                    {
                        facePhoto.Source = ImageSource.FromUri(new Uri(faceUrl));
                    }
                    if (user.Images.TryGetValue("id", out var idUrl) && !string.IsNullOrEmpty(idUrl))
                    {
                        idPhoto.Source = ImageSource.FromUri(new Uri(idUrl));
                    }
                    if (user.Images.TryGetValue("address", out var addressUrl) && !string.IsNullOrEmpty(addressUrl))
                    {
                        imgProofAddress.Source = ImageSource.FromUri(new Uri(addressUrl));
                    }
                    if (user.Images.TryGetValue("income", out var incomeUrl) && !string.IsNullOrEmpty(incomeUrl))
                    {
                        imgProofIncome.Source = ImageSource.FromUri(new Uri(incomeUrl));
                    }
                    if (user.Images.TryGetValue("payroll", out var payrollUrl) && !string.IsNullOrEmpty(payrollUrl))
                    {
                        imgProofPayroll.Source = ImageSource.FromUri(new Uri(payrollUrl));
                    }
                    if (user.Images.TryGetValue("accountStatus", out var accStatusUrl) && !string.IsNullOrEmpty(accStatusUrl))
                    {
                        imgProofAccStatus.Source = ImageSource.FromUri(new Uri(accStatusUrl));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error instead of showing an alert
                Console.WriteLine($"Failed to load user images: {ex.Message}");
            }
        }

        private async void OnUpdateFacePhotoClicked(object sender, EventArgs e)
        {
            await UpdatePhoto("face", facePhoto);
        }

        private async void OnUpdateIdPhotoClicked(object sender, EventArgs e)
        {
            await UpdatePhoto("id", idPhoto);
        }

        private async void OnUpdateAddressPhotoClicked(object sender, EventArgs e)
        {
            await UpdatePhoto("address", imgProofAddress);
        }

        private async void OnUpdateIncomePhotoClicked(object sender, EventArgs e)
        {
            await UpdatePhoto("income", imgProofIncome);
        }

        private async void OnUpdateProofPayroll(object sender, EventArgs e)
        {
            await UpdatePhoto("payroll", imgProofPayroll);
        }

        private async void OnUpdateProofAccStatus(object sender, EventArgs e)
        {
            await UpdatePhoto("accountStatus", imgProofAccStatus);
        }

        private async Task UpdatePhoto(string imageType, Image imageControl)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    var url = await UploadImage(photo, imageControl, imageType);
                    if (url != null)
                    {
                        if (currentUser.Images == null)
                            currentUser.Images = new Dictionary<string, string>();
                        currentUser.Images[imageType] = url;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update photo: {ex.Message}", "OK");
            }
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                currentUser.FirstName = firstNameEntry.Text;
                currentUser.LastName = lastNameEntry.Text;
                currentUser.PhoneNumber = phoneNumberEntry.Text;

                await _firebaseConnection.UpdateUserDataAsync(userId, currentUser);
                await DisplayAlert("Éxito", "Los datos del usuario se han actualizado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save changes: {ex.Message}", "OK");
            }
        }

        private async Task<string> UploadImage(FileResult photo, Image imageControl, string imageType)
        {
            try
            {
                if (photo != null)
                {
                    using (var stream = await photo.OpenReadAsync())
                    {
                        var fileName = $"{imageType}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                        var url = await _firebaseConnection.UploadImageAsync(userId, stream, fileName);
                        imageControl.Source = url;
                        return url;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to upload image: {ex.Message}", "OK");
            }
            return null;
        }
    }
}