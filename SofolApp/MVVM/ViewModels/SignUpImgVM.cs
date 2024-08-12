using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

namespace SofolApp.MVVM.ViewModels
{
    public partial class SignUpImgVM : ObservableObject
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private string _userId;

        [ObservableProperty]
        private UploadFlags _uploadFlags;

        [ObservableProperty]
        private ObservableCollection<UploadItem> _uploadItems;

        public SignUpImgVM(IFirebaseConnection firebaseConnection)
        {
            _firebaseConnection = firebaseConnection;
            UploadFlags = new UploadFlags();
            UploadItems = new ObservableCollection<UploadItem>
            {
                new UploadItem { Type = "face", Label = "Face photo not uploaded", TextColor = Colors.Red },
                new UploadItem { Type = "id", Label = "ID photo not uploaded", TextColor = Colors.Red },
                new UploadItem { Type = "address", Label = "Address proof not uploaded", TextColor = Colors.Red },
                new UploadItem { Type = "income", Label = "Income proof not uploaded", TextColor = Colors.Red },
                new UploadItem { Type = "payroll", Label = "Payroll not uploaded", TextColor = Colors.Red },
                new UploadItem { Type = "accountStatus", Label = "Account status not uploaded", TextColor = Colors.Red }
            };
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
                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_userId, stream, imageType, filePickerResult.FileName);
                        UpdateUploadFlags(imageType, true);

                        var user = await _firebaseConnection.ReadUserDataAsync(_userId);
                        if (user.Images == null)
                        {
                            user.Images = new Dictionary<string, string>();
                        }
                        user.Images[imageType] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_userId, user);

                        await Shell.Current.DisplayAlert("Success", $"{imageType} photo uploaded successfully.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading {imageType} photo: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to upload {imageType} photo: {ex.Message}", "OK");
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
                        string downloadUrl = await _firebaseConnection.UploadImageAsync(_userId, stream, "accountStatus", fileResult.FileName);
                        UpdateUploadFlags("accountStatus", true);

                        var user = await _firebaseConnection.ReadUserDataAsync(_userId);
                        if (user.Images == null)
                        {
                            user.Images = new Dictionary<string, string>();
                        }
                        user.Images["accountStatus"] = downloadUrl;
                        await _firebaseConnection.UpdateUserDataAsync(_userId, user);

                        await Shell.Current.DisplayAlert("Success", "Account statement PDF uploaded successfully.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading account status PDF: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to upload account statement PDF: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToReferences()
        {
            if (AllUploadsCompleted())
            {
                await Shell.Current.GoToAsync("//SignUpReferences");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Please upload all images and PDF files before continuing.", "OK");
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
                await Shell.Current.DisplayAlert("Error", "Please upload all images and PDF files before continuing.", "OK");
            }
        }

        private void UpdateUploadFlags(string imageType, bool isUploaded)
        {
            var item = UploadItems.FirstOrDefault(i => i.Type == imageType);
            if (item != null)
            {
                item.Label = isUploaded ? $"{char.ToUpper(imageType[0]) + imageType.Substring(1)} uploaded successfully" : $"{char.ToUpper(imageType[0]) + imageType.Substring(1)} not uploaded";
                item.TextColor = isUploaded ? Colors.Green : Colors.Red;
            }

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

    public partial class UploadItem : ObservableObject
    {
        public string Type { get; set; }

        [ObservableProperty]
        private string _label;

        [ObservableProperty]
        private Color _textColor;
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