using SofolApp.MVVM.ViewModels;
using System.Text.RegularExpressions;

namespace SofolApp.MVVM.Views;

public partial class ForgotPass : ContentPage
{
    private readonly FirebaseConnection firebaseConnection;

    public ForgotPass()
    {
        InitializeComponent();
        firebaseConnection = new FirebaseConnection();
    }

    private async void SendResetEmailClicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Por favor, ingrese su correo electrónico", "OK");
            return;
        }

        if (!IsValidEmail(email))
        {
            await DisplayAlert("Error", "Por favor, ingrese un correo electrónico válido", "OK");
            return;
        }

        try
        {
            SetLoadingState(true);
            await firebaseConnection.ResetPasswordAsync(email);
            await DisplayAlert("Éxito", "Se ha enviado un correo de restablecimiento de contraseña a tu dirección de correo electrónico.", "OK");
            await Navigation.PopAsync(); // Volver a la página anterior después de enviar el correo
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error detallado: {ex}");
            await DisplayAlert("Error", "No se pudo enviar el correo de restablecimiento. Por favor, inténtelo de nuevo más tarde.", "OK");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private bool IsValidEmail(string email)
    {
        // Patrón de expresión regular para validar el formato del correo electrónico
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private void SetLoadingState(bool isLoading)
    {
        activityIndicator.IsVisible = isLoading;
        activityIndicator.IsRunning = isLoading;
        sendButton.IsEnabled = !isLoading;
        emailEntry.IsEnabled = !isLoading;
    }
}