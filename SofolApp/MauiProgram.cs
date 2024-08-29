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

                // Configuration setup
                builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                // Azure Key Vault setup
                string keyVaultUri = builder.Configuration["KeyVaultUri"] ?? "Error en la carga de azure key vault";
                var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

                // Dependency injection setup
                builder.Services.AddSingleton<IAzureKeyVaultService>(sp => new AzureKeyVaultService(builder.Configuration));
                builder.Services.AddSingleton<IFirebaseConnection, FirebaseConnection>();

                builder.Services.AddSingleton<IAzureFaceService>(sp =>
                {
                    var keyVaultService = sp.GetRequiredService<IAzureKeyVaultService>();
                    var azureApiKey = keyVaultService.GetSecretAsync("AzureFaceApiKey").GetAwaiter().GetResult();
                    var azureEndpoint = keyVaultService.GetSecretAsync("AzureFaceEndpoint").GetAwaiter().GetResult();
                    return new AzureFaceService(azureApiKey, azureEndpoint);
                });

                // Sentry configuration
                builder.UseSentry(options =>
                {
                    options.Dsn = secretClient.GetSecret("SentryDsn").Value.Value;
                    options.Debug = true;
                    options.TracesSampleRate = 1.0;
                });

                // Other service registrations
                builder.Services.AddSingleton<IRegistrationStateService, RegistrationStateService>();
                builder.Services.AddTransient<SessionManager>();

                // View and ViewModel registrations
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