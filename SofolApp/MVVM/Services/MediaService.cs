using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Services
{ 
    public class MediaService : IMediaService
    {
        public async Task<FileResult> PickPhotoAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking photo: {ex.Message}");
            }
            return null;
        }
    }

}
