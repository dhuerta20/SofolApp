using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Media;

namespace SofolApp.MVVM.Services
{
    public interface IMediaService
    {
        Task<FileResult> PickPhotoAsync();
    }
}
