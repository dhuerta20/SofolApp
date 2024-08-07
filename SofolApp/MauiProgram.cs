using Microsoft.Extensions.Logging;
using Firebase.Auth;
using Firebase.Auth.Providers;
using SofolApp.MVVM.Views;
using SofolApp.Services;
using SofolApp.MVVM.ViewModels;
using Firebase.Auth.Repository;
using CommunityToolkit.Maui;
using SofolApp.Services;
using SofolApp.MVVM.ViewModels;
using SofolApp.MVVM.Services;

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
                        fonts.AddFont("Free-Solid-900.otf", "FAS");
                    });
                builder.Services.AddSingleton<IFirebaseConnection, FirebaseConnection>();
                builder.Services.AddTransient<SessionManager>();
                builder.Services.AddTransient<SignInVM>();
                builder.Services.AddTransient<SignInForm>();
                builder.Services.AddTransient<SignUpVM>();
                builder.Services.AddTransient<SignUpForm>();
                builder.Services.AddTransient<SignUpImgVM>();
                builder.Services.AddTransient<SignUpImg>();
                builder.Services.AddTransient<SignUpReferencesVM>();
                builder.Services.AddTransient<SignUpReferences>();
                builder.Services.AddSingleton<IMediaService, MediaService>();
                builder.Services.AddTransient<PersonalDataPageVM>();
                builder.Services.AddTransient<PersonalDataPage>();
                builder.Services.AddTransient<ReferencesPageVM>();
                builder.Services.AddTransient<ReferencesPage>();
                builder.Services.AddTransient<IdConfirmPageVM>();
                builder.Services.AddTransient<IdConfirmPage>();
                builder.Services.AddTransient<CreditPageVM>();
                builder.Services.AddTransient<CreditPage>();



#if DEBUG
                builder.Logging.AddDebug();
#endif

                return builder.Build();
            }
            catch(Exception ex) {
                Console.WriteLine($"Error en CreateMauiApp: {ex}");
                throw;
            }

        }
    }
}