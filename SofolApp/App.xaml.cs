using Microsoft.Maui.Controls;
using SofolApp.MVVM.Views;
using SofolApp.MVVM.ViewModels;
using SofolApp.Services;
using SofolApp.MVVM.Models;
using System;
using System.Threading.Tasks;

namespace SofolApp
{
    public partial class App : Application
    {
        private readonly IFirebaseConnection _firebaseConnection;
        private readonly IRegistrationStateService _registrationStateService;
        private readonly IAzureFaceService _azureFaceService;
        private readonly ILoadingService _loadingService;

        public App(IFirebaseConnection firebaseConnection,
                   IRegistrationStateService registrationStateService, IAzureFaceService azureFaceService,
                   ILoadingService loadingService)
        {
            try
            {
                InitializeComponent();
                InitializeSecretsAsync();
                _firebaseConnection = firebaseConnection;
                _registrationStateService = registrationStateService;
                _azureFaceService = azureFaceService;
                _loadingService = loadingService;
                MainPage = new AppShell();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en App constructor: {ex}");
                throw;
            }
        }

        private async void InitializeSecretsAsync()
        {
            await CustomSecretsManager.Instance.InitializeSecretsAsync();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled Exception: {e.ExceptionObject}");
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception}");
        }

        protected override async void OnStart()
        {
            try
            {
                base.OnStart();
                await HandleStartup();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en OnStart: {ex}");
            }
        }

        private async Task HandleStartup()
        {
            try
            {
                _loadingService.ShowLoadingAsync();
                var registrationState = await _registrationStateService.GetRegistrationStateAsync();

                if (!string.IsNullOrEmpty(registrationState))
                {
                    if (registrationState == "SignUpImg")
                    {
                        await Shell.Current.GoToAsync(nameof(SignUpImg));
                        return;
                    }
                    // Añadir más estados según sea necesario
                }

                await HandleAutoLogin();
            }
            finally
            {
                _loadingService.HideLoadingAsync();
            }
        }

        private async Task HandleAutoLogin()
        {
            try
            {
                var authToken = await SecureStorage.GetAsync("userToken");
                var userId = await SecureStorage.GetAsync("userId");

                if (!string.IsNullOrEmpty(authToken) && !string.IsNullOrEmpty(userId))
                {
                    bool canContinueSession = await SessionManager.CheckSessionCount();
                    if (canContinueSession)
                    {
                        var user = await _firebaseConnection.ReadUserDataAsync(userId);

                        if (user != null)
                        {
                            if (await SessionManager.ShouldPerformFacialRecognition())
                            {
                                await PerformFacialRecognition(user);
                            }
                            else
                            {
                                await Shell.Current.GoToAsync(nameof(CreditPage));
                            }
                        }
                        else
                        {
                            await _firebaseConnection.SignOutAsync();
                            await Shell.Current.GoToAsync(nameof(SignInForm));
                        }
                    }
                    else
                    {
                        await _firebaseConnection.SignOutAsync();
                        await Shell.Current.GoToAsync(nameof(SignInForm));
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(SignInForm));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el auto-login: {ex.Message}");
                await Shell.Current.GoToAsync(nameof(SignInForm));
            }
        }

        private async Task PerformFacialRecognition(Users user)
        {
            try
            {
                if (user.Images.TryGetValue("face", out string faceImageUrl))
                {
                    // Descargar la imagen de la URL
                    using (var httpClient = new HttpClient())
                    {
                        var imageStream = await httpClient.GetStreamAsync(faceImageUrl);

                        // Realizar el reconocimiento facial
                        bool isRecognized = await _azureFaceService.VerifyFaceAsync(imageStream);

                        if (isRecognized)
                        {
                            await SessionManager.UpdateLastFacialRecognition();
                            await Shell.Current.GoToAsync(nameof(CreditPage));
                        }
                        else
                        {
                            await _firebaseConnection.SignOutAsync();
                            await Shell.Current.DisplayAlert("Error", "Falló la verificación facial. Por favor, inicie sesión nuevamente.", "OK");
                            await Shell.Current.GoToAsync(nameof(SignInForm));
                        }
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se encontró una imagen facial registrada. Por favor, actualice su perfil.", "OK");
                    await Shell.Current.GoToAsync(nameof(SignInForm));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el reconocimiento facial: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error durante la verificación facial. Por favor, intente más tarde.", "OK");
                await Shell.Current.GoToAsync(nameof(SignInForm));
            }
        }
    }
}