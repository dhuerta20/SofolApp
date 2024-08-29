using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public class AzureKeyVaultService : IAzureKeyVaultService
    {
        private readonly SecretClient _secretClient;

        public AzureKeyVaultService(IConfiguration configuration)
        {
            string keyVaultUri = configuration["KeyVaultUri"];
            _secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving secret {secretName}: {ex.Message}");
                throw;
            }
        }
    }
}