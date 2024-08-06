using Microsoft.Maui.Controls;
using SofolApp.MVVM.Views;
using SofolApp.MVVM.ViewModels;
using System;
using System.Threading.Tasks;

namespace SofolApp
{
    public partial class App : Application
    {

        private readonly FirebaseConnection _firebaseConnection;

        public App()
        {
            // Breakpoint aquí
            try
            {
                InitializeComponent();
                _firebaseConnection = new FirebaseConnection();
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
            // Breakpoint aquí
            try
            {
                base.OnStart();
                await HandleAutoLogin();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en OnStart: {ex}");
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
                            await Shell.Current.GoToAsync("//CreditApp");
                        }
                        else
                        {
                            await _firebaseConnection.SignOutAsync();
                            await Shell.Current.GoToAsync("//SignInForm");
                        }
                    }
                    else
                    {
                        await _firebaseConnection.SignOutAsync();
                        await Shell.Current.GoToAsync("//SignInForm");
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync("//SignInForm");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during auto-login: {ex.Message}");
                await _firebaseConnection.SignOutAsync();
                await Shell.Current.GoToAsync("//SignInForm");
            }
        }
    }
}
