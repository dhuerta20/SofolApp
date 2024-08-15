using System;
using System.Threading.Tasks;
using Microsoft.Maui.Media;

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

        public async Task<FileResult> CapturePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.Default.CapturePhotoAsync();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing photo: {ex.Message}");
            }
            return null;
        }
    }
}
