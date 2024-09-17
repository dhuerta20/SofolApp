using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public class AzureFaceService : IAzureFaceService
    {
        private IFaceClient _faceClient;

        public AzureFaceService()
        {
            // No necesitamos inyectar CustomSecretsManager aquí
        }

        private async Task InitializeAsync()
        {
            if (_faceClient == null)
            {
                var secretsManager = CustomSecretsManager.Instance;
                var apiKey = await secretsManager.GetAzureFaceApiKeyAsync();
                var endpoint = await secretsManager.GetAzureFaceEndpointAsync();
                _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(apiKey))
                {
                    Endpoint = endpoint
                };
            }
        }

        public async Task<bool> VerifyFaceAsync(Stream imageStream)
        {
            try
            {
                await InitializeAsync();
                Console.WriteLine("Llamando a Azure Face API");
                var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(imageStream);
                Console.WriteLine($"Rostros detectados: {detectedFaces.Count}");
                return detectedFaces.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Azure Face API: {ex.Message}");
                return false;
            }
        }
    }
}