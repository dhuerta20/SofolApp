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
            await DisplayAlert("Error", "Por favor, ingrese su correo electr�nico", "OK");
            return;
        }

        if (!IsValidEmail(email))
        {
            await DisplayAlert("Error", "Por favor, ingrese un correo electr�nico v�lido", "OK");
            return;
        }

        try
        {
            SetLoadingState(true);
            await firebaseConnection.ResetPasswordAsync(email);
            await DisplayAlert("�xito", "Se ha enviado un correo de restablecimiento de contrase�a a tu direcci�n de correo electr�nico.", "OK");
            await Navigation.PopAsync(); // Volver a la p�gina anterior despu�s de enviar el correo
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error detallado: {ex}");
            await DisplayAlert("Error", "No se pudo enviar el correo de restablecimiento. Por favor, int�ntelo de nuevo m�s tarde.", "OK");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private bool IsValidEmail(string email)
    {
        // Patr�n de expresi�n regular para validar el formato del correo electr�nico
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