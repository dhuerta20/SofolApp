using Microsoft.Extensions.Logging;
using Firebase.Auth;
using Firebase.Auth.Providers;
using SofolApp.MVVM.Views;
using SofolApp.MVVM.ViewModels;
using Firebase.Auth.Repository;
using CommunityToolkit.Maui;

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