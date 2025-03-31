using System;
using System.IO;

public class Log
{
    private string logFilePath;
    private static Log instance;

  
    private Log(string path)
    {
        logFilePath = path;
    }

    public static Log GetInstance(string path)
    {
        if (instance == null)
        {
            instance = new Log(path);
        }
        return instance;
    }

    // Método para registrar una acción
    public void RegistrarAccion(string usuarioAdmin, string accion)
    {
        string fechaHora = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        string logMessage = $"{fechaHora} - Usuario: {usuarioAdmin} - Acción: {accion}";

        // Escribir la acción en el archivo de log
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}
