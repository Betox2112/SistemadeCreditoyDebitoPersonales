using System;
using System.IO;

namespace SistemaDeCreditoYDebitoPersonales.Model
{
    // Clase para manejar el registro de acciones en un archivo de log
    public class Log
    {
        private string logFilePath;
        private static Log instance;

        // Constructor privado para implementar el patrón Singleton
        private Log(string path)
        {
            logFilePath = path;
        }

        // Método para obtener la única instancia de Log (Singleton)
        public static Log GetInstance(string path)
        {
            if (instance == null)
            {
                instance = new Log(path);
            }
            return instance;
        }

        // Método para registrar una acción en el archivo de log
        public void RegistrarAccion(string usuarioAdmin, string accion)
        {
            try
            {
                string fechaHora = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string logMessage = $"{fechaHora} - Usuario: {usuarioAdmin} - Acción: {accion}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error al registrar en el log: {ex.Message}");
            }
        }
    }
}