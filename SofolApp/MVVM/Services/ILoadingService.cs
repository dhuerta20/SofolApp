using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public interface ILoadingService
    {
        Task ShowLoadingAsync();
        Task HideLoadingAsync();
    }
}
