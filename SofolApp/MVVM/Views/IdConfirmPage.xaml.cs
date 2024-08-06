using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;
namespace SofolApp.MVVM.Views;

public partial class IdConfirmPage : ContentPage
{
    private FirebaseConnection _firebaseConnection;

    public IdConfirmPage()
    {
        InitializeComponent();
        _firebaseConnection = new FirebaseConnection();
        LoadUserStatus();
    }

    private async void LoadUserStatus()
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
                        ProfileImage.Source = imageEntry.Value;
                    }
                }
            }

            UserNameLabel.Text = $"{user.FirstName}";

            switch (user.IsValid.ToLower())
            {
                case "approved":
                    StatusImage.Source = "aproved.png";
                    StatusLabel.Text = "Aprobado";
                    StatusLabel.TextColor = Colors.Green;
                    break;
                case "disapproved":
                    StatusImage.Source = "disaproved.png";
                    StatusLabel.Text = "No aprobado";
                    StatusLabel.TextColor = Colors.Red;
                    break;
                default:
                    StatusImage.Source = "pending.png";
                    StatusLabel.Text = "Pendiente";
                    StatusLabel.TextColor = Colors.Orange;
                    break;
            }

            AdminNotesLabel.Text = user.AdminNotes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar los datos del usuario: {ex.Message}", "OK");
        }
    }

    protected override bool OnBackButtonPressed()
    {
        NavigateBack();
        return true;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Esto manejará el gesto de deslizar hacia atrás en iOS
        NavigationPage.SetHasBackButton(this, true);
        NavigationPage.SetBackButtonTitle(this, "");
    }

    private async void NavigateBack()
    {
        await Shell.Current.GoToAsync("//CreditApp");
    }
}