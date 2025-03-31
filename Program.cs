using System;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Configurar codificación a UTF-8
        Sistema sistema = new Sistema();
        sistema.Iniciar();
    }
}
