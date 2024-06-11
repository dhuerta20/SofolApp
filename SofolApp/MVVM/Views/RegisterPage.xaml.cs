namespace SofolApp.MVVM.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void idPhotoClicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.CapturePhotoAsync();
        if (foto != null)
        {
            var memoryStream = await foto.OpenReadAsync();
            idPhoto.Source = ImageSource.FromStream(() => memoryStream);
            idPhoto.IsVisible = true;
        }
    }

    private async void proofAddressClicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.CapturePhotoAsync();
        if (foto != null)
        {
            var memoryStream = await foto.OpenReadAsync();
            imgProofAddress.Source = ImageSource.FromStream(() => memoryStream);
            imgProofAddress.IsVisible = true;
        }
    }

    private async void proofIncomeClicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.CapturePhotoAsync();
        if (foto != null)
        {
            var memoryStream = await foto.OpenReadAsync();
            imgProofIncome.Source = ImageSource.FromStream(() => memoryStream);
            imgProofIncome.IsVisible = true;
        }
    }

    private void ShowFormButtonClicked(object sender, EventArgs e)
    {
        FormLayout.IsVisible = true;
        ShowFormButton.IsVisible = false;
    }
}