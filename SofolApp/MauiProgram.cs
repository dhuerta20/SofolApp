using Microsoft.Extensions.Logging;
using Firebase.Auth;
using Firebase.Auth.Providers;
using SofolApp.MVVM.Views;
using SofolApp.Services;
using SofolApp.MVVM.ViewModels;
using Firebase.Auth.Repository;
using CommunityToolkit.Maui;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SofolApp
    {
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .UseMauiCommunityToolkit()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                        fonts.AddFont("FontAwesomeSolid-900", "FAS");
                    });

                // Registrar FirebaseConnection
                builder.Services.AddSingleton<IFirebaseConnection, FirebaseConnection>();

                // Registrar AzureFaceService
                builder.Services.AddSingleton<IAzureFaceService, AzureFaceService>();

                // Otros servicios
                builder.Services.AddSingleton<IRegistrationStateService, RegistrationStateService>();
                builder.Services.AddSingleton<ILoadingService, LoadingService>();

                // Carga de las Vistas y los ViewModels
                builder.Services.AddTransient<SessionManager>();
                builder.Services.AddTransient<SignInVM>();
                builder.Services.AddTransient<SignInForm>();
                builder.Services.AddTransient<ForgotPassVM>();
                builder.Services.AddTransient<ForgotPass>();
                builder.Services.AddTransient<SignUpVM>();
                builder.Services.AddTransient<SignUpForm>();
                builder.Services.AddTransient<SignUpImgVM>();
                builder.Services.AddTransient<SignUpImg>();
                builder.Services.AddTransient<SignUpReferencesVM>();
                builder.Services.AddTransient<SignUpReferences>();
                builder.Services.AddTransient<PersonalDataPageVM>();
                builder.Services.AddTransient<PersonalDataPage>();
                builder.Services.AddTransient<ReferencesPageVM>();
                builder.Services.AddTransient<ReferencesPage>();
                builder.Services.AddTransient<IdConfirmPageVM>();
                builder.Services.AddTransient<IdConfirmPage>();
                builder.Services.AddTransient<CreditPageVM>();
                builder.Services.AddTransient<CreditPage>();

                // Logging configuration
#if DEBUG
                builder.Logging.AddDebug();
#endif

                    return builder.Build();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en CreateMauiApp: {ex}");
                    throw;
                }
            }
        }
    }