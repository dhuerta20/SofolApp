using Firebase.Auth;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SofolApp.MVVM.Models;

namespace SofolApp.Services
{
    public interface IFirebaseConnection
    {
        Task<UserCredential> SignInAsync(string email, string password);
        Task SignOutAsync();
        Task<UserCredential> CreateUserAsync(string email, string password, string firstName, string lastName, string phoneNumber);
        Task<Users> ReadUserDataAsync(string userId);
        Task UpdateUserDataAsync(string userId, Users userData);
        Task<bool> CheckIfUserExistsAsync(string email);
        Task<string> UploadImageAsync(string userId, Stream imageStream, string fileName);
        Task<string> UploadPdfAsync(string userId, Stream pdfStream, string fileName);
        Task AddReferenceAsync(string userId, string referenceEmail);
        Task<List<string>> GetReferencesAsync(string userId);
        Task UpdateReferencesAsync(string userId, string firstReference, string secondReference, string thirdReference);
        Task ResetPasswordAsync(string email);
    }
}