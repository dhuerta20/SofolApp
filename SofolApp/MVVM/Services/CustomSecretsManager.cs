using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SofolApp.Services
{
    public class CustomSecretsManager
    {
        private static readonly Lazy<CustomSecretsManager> _instance = new Lazy<CustomSecretsManager>(() => new CustomSecretsManager());
        private readonly HttpClient _httpClient;
        private const string TIMESTAMP_KEY = "LAST_UPDATE_TIME";
        private const int REFRESH_INTERVAL_HOURS = 24;
        private const string ENCRYPTION_KEY = "ENCRYPTION_KEY";
        private readonly string _baseUrl = "https://proxcreditmobilewebapi-ceekhkeca6cbf7g8.eastus2-01.azurewebsites.net/api/KeyVault";

        private readonly List<string> _requiredSecrets = new List<string>
        {
            "AZUREFACEAPIKEY",
            "AZUREFACEENDPOINT",
            "FIREBASEAPIKEY",
            "FIREBASEAUTHDOMAIN",
            "FIREBASEDBURL",
            "FIREBASESTORAGEBUCKET"
        };

        public static CustomSecretsManager Instance => _instance.Value;

        private CustomSecretsManager()
        {
            _httpClient = new HttpClient();
        }

        public async Task InitializeSecretsAsync()
        {
            if (await ShouldRefreshSecrets())
            {
                await FetchAndStoreSecrets();
            }
        }

        private async Task<bool> ShouldRefreshSecrets()
        {
            var lastUpdateStr = await SecureStorage.GetAsync(TIMESTAMP_KEY);
            if (string.IsNullOrEmpty(lastUpdateStr)) return true;
            if (!DateTime.TryParse(lastUpdateStr, out var lastUpdate)) return true;
            return (DateTime.UtcNow - lastUpdate).TotalHours >= REFRESH_INTERVAL_HOURS;
        }

        private async Task FetchAndStoreSecrets()
        {
            try
            {
                foreach (var secretName in _requiredSecrets)
                {
                    var secretValue = await FetchSecret(secretName);
                    if (!string.IsNullOrEmpty(secretValue))
                    {
                        await SetEncryptedSecretAsync(secretName, secretValue);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch secret: {secretName}");
                    }
                }
                await SecureStorage.SetAsync(TIMESTAMP_KEY, DateTime.UtcNow.ToString("O"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching secrets: {ex.Message}");
                throw;
            }
        }

        private async Task<string> FetchSecret(string secretName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/secret/{secretName}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"Failed to fetch secret {secretName}. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching secret {secretName}: {ex.Message}");
                return null;
            }
        }

        private async Task SetEncryptedSecretAsync(string key, string value)
        {
            var encryptionKey = await GetOrCreateEncryptionKey();
            var encryptedValue = Encrypt(value, encryptionKey);
            await SecureStorage.SetAsync(key, encryptedValue);
        }

        private async Task<string> GetOrCreateEncryptionKey()
        {
            var key = await SecureStorage.GetAsync(ENCRYPTION_KEY);
            if (string.IsNullOrEmpty(key))
            {
                key = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                await SecureStorage.SetAsync(ENCRYPTION_KEY, key);
            }
            return key;
        }

        private string Encrypt(string clearText, string encryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText, string encryptionKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            await InitializeSecretsAsync();
            var encryptedValue = await SecureStorage.GetAsync(secretName);
            if (string.IsNullOrEmpty(encryptedValue)) return null;
            var encryptionKey = await GetOrCreateEncryptionKey();
            return Decrypt(encryptedValue, encryptionKey);
        }

        // Properties for quick access to common secrets
        public Task<string> GetAzureFaceApiKeyAsync() => GetSecretAsync("AZUREFACEAPIKEY");
        public Task<string> GetAzureFaceEndpointAsync() => GetSecretAsync("AZUREFACEENDPOINT");
        public Task<string> GetFirebaseApiKeyAsync() => GetSecretAsync("FIREBASEAPIKEY");
        public Task<string> GetFirebaseAuthDomainAsync() => GetSecretAsync("FIREBASEAUTHDOMAIN");
        public Task<string> GetFirebaseDbUrlAsync() => GetSecretAsync("FIREBASEDBURL");
        public Task<string> GetFirebaseStorageBucketAsync() => GetSecretAsync("FIREBASESTORAGEBUCKET");
    }
}