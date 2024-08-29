using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public class RegistrationStateService : IRegistrationStateService
    {
        private const string RegistrationStateKey = "RegistrationState";

        public async Task SaveRegistrationStateAsync(string state)
        {
            await SecureStorage.SetAsync(RegistrationStateKey, state);
        }

        public async Task<string> GetRegistrationStateAsync()
        {
            return await SecureStorage.GetAsync(RegistrationStateKey);
        }

        public Task ClearRegistrationStateAsync()
        {
            SecureStorage.Remove(RegistrationStateKey);
            return Task.CompletedTask;
        }
    }
}