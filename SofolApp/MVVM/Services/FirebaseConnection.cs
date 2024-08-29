using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using SofolApp.MVVM.Models;
using SofolApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public class FirebaseConnection : IFirebaseConnection
    {
        private const string ApiKey = "AIzaSyBUqYQowXYaMJYpnLM4wu5b4YNk7Iw8LDk";
        private const string AuthDomain = "creditapptest-c8a1d.firebaseapp.com";
        private const string DatabaseUrl = "https://creditapptest-c8a1d-default-rtdb.firebaseio.com/";
        private const string StorageBucket = "creditapptest-c8a1d.appspot.com";

        // Firebase Authentication
        public FirebaseAuthClient ConnectToFirebase()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = ApiKey,
                AuthDomain = AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new GoogleProvider().AddScopes("email"),
                    new EmailProvider()
                },
            };

            return new FirebaseAuthClient(config);
        }

        // ---------------------------Firebase Realtime Database------------------------------
        public static FirebaseClient GetDatabaseClient()
        {
            return new FirebaseClient(DatabaseUrl);
        }

        // ----------------------------------Authentication Methods---------------------------
        public async Task<UserCredential> SignInAsync(string email, string password)
        {
            try
            {
                var client = ConnectToFirebase();
                var userCredential = await client.SignInWithEmailAndPasswordAsync(email, password);
                await SecureStorage.SetAsync("userId", userCredential.User.Uid);
                await SecureStorage.SetAsync("userToken", await userCredential.User.GetIdTokenAsync());
                return userCredential;
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Authentication error: {ex.Reason}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during sign in", ex);
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
 
                //Clear user data from SecureStorage
                SecureStorage.Remove("userId");
                SecureStorage.Remove("userToken");
                
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during sign out", ex);
            }
        }

        public async Task<UserCredential> CreateUserAsync(string email, string password, string firstName, string lastName, string phoneNumber)
        {
            try
            {
                var client = ConnectToFirebase();
                var userCredential = await client.CreateUserWithEmailAndPasswordAsync(email, password, $"{firstName} {lastName}");
                await SecureStorage.SetAsync("userId", userCredential.User.Uid);
                await SecureStorage.SetAsync("userToken", await userCredential.User.GetIdTokenAsync());

                var userData = new Users
                {
                    userId = userCredential.User.Uid,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    IsValid = "pending", // Set to "pending" by default
                    IsAdmin = false,
                    Images = new Dictionary<string, string>(),
                    FirstReference = "",
                    SecondReference = "",
                    ThirdReference = "",
                    AdminNotes = "" // Initialize AdminNotes as an empty string
                };
                await CreateUserDataAsync(userCredential.User.Uid, userData);

                return userCredential;
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Error creating user: {ex.Reason}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error creating user", ex);
            }
        }

        // -------------------------------Database Operations------------------------
        public async Task CreateUserDataAsync(string userId, Users userData)
        {
            var client = GetDatabaseClient();
            await client.Child("users").Child(userId).PutAsync(userData);
        }

        public async Task<Users> ReadUserDataAsync(string userId)
        {
            var client = GetDatabaseClient();
            return await client.Child("users").Child(userId).OnceSingleAsync<Users>();
        }

        public async Task UpdateUserDataAsync(string userId, Users userData)
        {
            var client = GetDatabaseClient();
            await client.Child("users").Child(userId).PutAsync(userData);
        }

        public async Task<bool> CheckIfUserExistsAsync(string email)
        {
            var client = GetDatabaseClient();
            var users = await client.Child("users")
                .OrderBy("Email")
                .EqualTo(email)
                .OnceAsync<Users>();

            return users.Any();
        }

        // --------------------------Image Upload-------------------------

        private string SanitizeFileName(string fileName)
        {
            // Replace invalid characters with underscores
            return System.Text.RegularExpressions.Regex.Replace(fileName, @"[^\w\-]", "_");
        }

        public async Task<string> UploadImageAsync(string userId, Stream imageStream, string imageType)
        {
            var fileName = $"{imageType}.jpg"; // Usar un nombre de archivo fijo basado en el tipo de imagen
            var storage = new FirebaseStorage(StorageBucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => SecureStorage.GetAsync("userToken")
            });
            var imageUrl = await storage
                .Child("user_images")
                .Child(userId)
                .Child(fileName)
                .PutAsync(imageStream);

            return imageUrl;
        }
        public async Task<string> UploadPdfAsync(string userId, Stream pdfStream, string fileName)
        {
            var sanitizedFileName = SanitizeFileName(fileName);
            var storage = new FirebaseStorage(StorageBucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => SecureStorage.GetAsync("userToken")
            });

            var pdfUrl = await storage
                .Child("user_images")
                .Child(userId)
                .Child(sanitizedFileName)
                .PutAsync(pdfStream);

            return pdfUrl;
        }

        //--------------------REFERENCES-------------------------

        public async Task AddReferenceAsync(string userId, string referenceEmail)
        {
            var user = await ReadUserDataAsync(userId);

            if (string.IsNullOrEmpty(user.FirstReference))
            {
                user.FirstReference = referenceEmail;
            }
            else if (string.IsNullOrEmpty(user.SecondReference))
            {
                user.SecondReference = referenceEmail;
            }
            else if (string.IsNullOrEmpty(user.ThirdReference))
            {
                user.ThirdReference = referenceEmail;
            }
            else
            {
                throw new Exception("Ya se han agregado todas las referencias posibles.");
            }

            await UpdateUserDataAsync(userId, user);
        }

        public async Task<List<string>> GetReferencesAsync(string userId)
        {
            var user = await ReadUserDataAsync(userId);
            var references = new List<string>
            {
                user.FirstReference ?? "",
                user.SecondReference ?? "",
                user.ThirdReference ?? ""
            };
            return references.Where(r => !string.IsNullOrEmpty(r)).ToList();
        }

        public async Task UpdateReferencesAsync(string userId, string firstReference, string secondReference, string thirdReference)
        {
            var user = await ReadUserDataAsync(userId);
            user.FirstReference = firstReference;
            user.SecondReference = secondReference;
            user.ThirdReference = thirdReference;
            await UpdateUserDataAsync(userId, user);
        }

        // Password Reset
        public async Task ResetPasswordAsync(string email)
        {
            try
            {
                var client = ConnectToFirebase();
                await client.ResetEmailPasswordAsync(email);
                Console.WriteLine("Correo de restablecimiento de contraseña enviado.");
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Error enviando correo de restablecimiento de contraseña: {ex.Reason}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado al enviar correo de restablecimiento de contraseña", ex);
            }
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var client = ConnectToFirebase();
                await client.ResetEmailPasswordAsync(email);
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Error al enviar correo de restablecimiento: {ex.Reason}", ex);
            }
        }
    }
}