using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public interface IRegistrationStateService
    {
        Task SaveRegistrationStateAsync(string state);
        Task<string> GetRegistrationStateAsync();
        Task ClearRegistrationStateAsync();
    }
}
