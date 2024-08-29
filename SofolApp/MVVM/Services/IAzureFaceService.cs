using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.Services
{
   public interface IAzureFaceService
    {
        Task<bool> VerifyFaceAsync(Stream imageStream);
    }
    
}