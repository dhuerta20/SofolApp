using SofolApp.MVVM.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpImg : ContentPage
    {
        private readonly FirebaseConnection _firebaseConnection;
        private string _userId;
        private readonly UploadFlags _uploadFlags;

        public SignUpImg()
        {
            InitializeComponent();
            _firebaseConnection = new FirebaseConnection();
            _uploadFlags = new UploadFlags();

        }

        private void ResetUploadFlags()
        {
            _uploadFlags.FaceUploaded = false;
            _uploadFlags.IdUploaded = false;
            _uploadFlags.AddressUploaded = false;
            _uploadFlags.IncomeUploaded = false;
            _uploadFlags.PayrollUploaded = false;
            _uploadFlags.AccountStatusUploaded = false;

            // Reset labels
            lblFacePhoto.Text = "Face photo not uploaded";
            lblIDPhoto.Text = "ID photo not uploaded";
            lblAddressProof.Text = "Address proof not uploaded";
            lblIncomeProof.Text = "Income proof not uploaded";
            lblPayroll.Text = "Payroll not uploaded";
            lblAccountStatus.Text = "Account status not uploaded";

            // Reset label colors
            lblFacePhoto.TextColor = Colors.Red;
            lblIDPhoto.TextColor = Colors.Red;
            lblAddressProof.TextColor = Colors.Red;
            lblIncomeProof.TextColor = Colors.Red;
            lblPayroll.TextColor = Colors.Red;
            lblAccountStatus.TextColor = Colors.Red;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _userId = await SecureStorage.GetAsync("userId");
        }


        private async void OnFacePhotoButtonClicked(object sender, EventArgs e) => await UploadImageAsync("face");

        private async void OnIDPhotoButtonClicked(object sender, EventArgs e) => await UploadImageAsync("id");

        private async void OnAddressProofButtonClicked(object sender, EventArgs e) => await UploadImageAsync("address");

        private async void OnIncomeProofButtonClicked(object sender, EventArgs e) => await UploadImageAsync("income");

        private async void OnPayrollButtonClicked(object sender, EventArgs e) => await UploadImageAsync("payroll");

        private async void OnAccountStatusButtonClicked(object sender, EventArgs e)
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
                    PickerTitle = "Seleccione su estado de cuenta en PDF"
                });

                if (fileResult != null)
                {
                    using (var stream = await fileResult.OpenReadAsync())
                    {
                        string downloadUrl = await _firebaseConnection.UploadPdfAsync(_userId, stream, $"accountStatus_{fileResult.FileName}");
                        UpdateUploadFlags("accountStatus", true);

                        // Actualizar la base de datos con la URL del PDF
                        var user = await _firebaseConnection.ReadUserDataAsync(_userId);
                        if (user.Images == null)
                        {
                            user.Images = new Dictionary<string, string>();
                        }
                        user.Images["accountStatus"] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_userId, user);

                        lblAccountStatus.Text = "Estado de cuenta subido con éxito";
                        lblAccountStatus.TextColor = Colors.Green;
                        await DisplayAlert("Éxito", "Estado de cuenta en PDF subido correctamente.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading account status PDF: {ex.Message}");
                await DisplayAlert("Error", $"No se pudo subir el estado de cuenta en PDF: {ex.Message}", "OK");
            }
        }

        private async void SignUpReferencesButton(object sender, EventArgs e)
        {
            if (AllUploadsCompleted())
            {
                await Shell.Current.GoToAsync("//SignUpReferences");
            }
            else
            {
                await DisplayAlert("Error", "Por favor, sube todas las imágenes y archivos PDF antes de continuar.", "OK");
            }
        }

        private async void OnEndOfRegisterButtonClicked(object sender, EventArgs e)
        {
            if (AllUploadsCompleted())
            {
                await Shell.Current.GoToAsync("//CreditApp");
            }
            else
            {
                await DisplayAlert("Error", "Por favor, sube todas las imágenes y archivos PDF antes de continuar.", "OK");
            }
        }

        private async Task UploadImageAsync(string imageType)
        {
            try
            {
                var filePickerResult = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = $"Select {imageType} photo",
                    FileTypes = FilePickerFileType.Images
                });

                if (filePickerResult != null && !string.IsNullOrEmpty(_userId))
                {
                    using (var stream = await filePickerResult.OpenReadAsync())
                    {
                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_userId, stream, $"{imageType}_{filePickerResult.FileName}");
                        UpdateUploadFlags(imageType, true);

                        // Update the database with the image URL
                        var user = await _firebaseConnection.ReadUserDataAsync(_userId);
                        if (user.Images == null)
                        {
                            user.Images = new Dictionary<string, string>();
                        }
                        user.Images[imageType] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_userId, user);

                        await DisplayAlert("Success", $"{imageType} photo uploaded successfully.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "User is not authenticated or no image was selected.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading {imageType} photo: {ex.Message}");
                await DisplayAlert("Error", $"Failed to upload {imageType} photo: {ex.Message}", "OK");
            }
        }

        private void UpdateUploadFlags(string imageType, bool isUploaded)
        {
            switch (imageType)
            {
                case "face":
                    _uploadFlags.FaceUploaded = isUploaded;
                    lblFacePhoto.Text = "Image uploaded successfully";
                    lblFacePhoto.TextColor = Colors.Green;
                    break;
                case "id":
                    _uploadFlags.IdUploaded = isUploaded;
                    lblIDPhoto.Text = "Image uploaded successfully";
                    lblIDPhoto.TextColor = Colors.Green;
                    break;
                case "address":
                    _uploadFlags.AddressUploaded = isUploaded;
                    lblAddressProof.Text = "Image uploaded successfully";
                    lblAddressProof.TextColor = Colors.Green;
                    break;
                case "income":
                    _uploadFlags.IncomeUploaded = isUploaded;
                    lblIncomeProof.Text = "Image uploaded successfully";
                    lblIncomeProof.TextColor = Colors.Green;
                    break;
                case "payroll":
                    _uploadFlags.PayrollUploaded = isUploaded;
                    lblPayroll.Text = "Image uploaded successfully";
                    lblPayroll.TextColor = Colors.Green;
                    break;
                case "accountStatus":
                    _uploadFlags.AccountStatusUploaded = isUploaded;
                    lblAccountStatus.Text = "PDF uploaded successfully";
                    lblAccountStatus.TextColor = Colors.Green;
                    break;
            }
        }

        private bool AllUploadsCompleted()
        {
            return _uploadFlags.FaceUploaded &&
                   _uploadFlags.IdUploaded &&
                   _uploadFlags.AddressUploaded &&
                   _uploadFlags.IncomeUploaded &&
                   _uploadFlags.PayrollUploaded &&
                   _uploadFlags.AccountStatusUploaded;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//SignUpForm");
            });
            return true;
        }
    }

    public class UploadFlags
    {
        public bool FaceUploaded { get; set; }
        public bool IdUploaded { get; set; }
        public bool AddressUploaded { get; set; }
        public bool IncomeUploaded { get; set; }
        public bool PayrollUploaded { get; set; }
        public bool AccountStatusUploaded { get; set; }
    }
}
