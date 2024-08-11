/*using System;
using System.Threading.Tasks;

namespace SofolApp.MVVM.ViewModels
{
    public class SessionManager
    {
        private const string SessionCountKey = "SessionCount";
        private const int MaxSessionCount = 15;

        public static async Task<bool> CheckSessionCount()
        {
            try
            {
                int sessionCount = await GetSessionCount();

                if (sessionCount >= MaxSessionCount)
                {
                    await ResetSessionCount();
                    return false;
                }

                await IncrementSessionCount();
                return true;
            }
            catch (Exception ex)
            {
                // Manejar el error aquí (e.g., registrar el error o notificar al usuario)
                Console.WriteLine($"Error al verificar el conteo de sesiones: {ex.Message}");
                return false;
            }
        }

        public static async Task<int> GetSessionCount()
        {
            try
            {
                string countStr = await SecureStorage.GetAsync(SessionCountKey) ?? "0";
                return int.TryParse(countStr, out int count) ? count : 0;
            }
            catch (Exception ex)
            {
                // Manejar el error aquí (e.g., registrar el error o notificar al usuario)
                Console.WriteLine($"Error al obtener el conteo de sesiones: {ex.Message}");
                return 0;
            }
        }

        public static async Task ResetSessionCount()
        {
            try
            {
                await SecureStorage.SetAsync(SessionCountKey, "1");
            }
            catch (Exception ex)
            {
                // Manejar el error aquí (e.g., registrar el error o notificar al usuario)
                Console.WriteLine($"Error al restablecer el conteo de sesiones: {ex.Message}");
            }
        }

        public static async Task IncrementSessionCount()
        {
            try
            {
                int currentCount = await GetSessionCount();
                await SecureStorage.SetAsync(SessionCountKey, (currentCount + 1).ToString());
            }
            catch (Exception ex)
            {
                // Manejar el error aquí (e.g., registrar el error o notificar al usuario)
                Console.WriteLine($"Error al incrementar el conteo de sesiones: {ex.Message}");
            }
        }
    }
}
*/