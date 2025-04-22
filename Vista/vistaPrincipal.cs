using System;

namespace SistemaDeCreditoYDebitoPersonales.Vista
{
    // Clase para mostrar mensajes y menús al usuario
    public class vistaPrincipal
    {
        // Constructor vacío
        public vistaPrincipal()
        {
        }

        // Método para mostrar un mensaje de éxito
        private void MostrarMensajeExito(string mensaje)
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"  {mensaje}");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("  Presiona ENTER para continuar...");
            Console.ReadLine();
        }

        // Método para mostrar el menú principal y capturar la opción del usuario
        public string MostrarMenu()
        {
            string opcionElegida;

            // Usamos un do-while para asegurarnos de que el usuario ingrese una opción válida
            do
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("         MENÚ PRINCIPAL            ");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("  1. Registrar nuevo cliente");
                Console.WriteLine("  2. Mostrar todos los clientes");
                Console.WriteLine("  3. Mostrar clientes que deben");
                Console.WriteLine("  4. Buscar cliente por cédula");
                Console.WriteLine("  5. Gestión de Créditos");
                Console.WriteLine("  6. Gestión de Débitos");
                Console.WriteLine("  7. Gestión de Préstamos Personales");
                Console.WriteLine("  8. Guardar en archivo (.txt)");
                Console.WriteLine("  9. Cargar desde archivo");
                Console.WriteLine("  10. Cerrar sesión");
                Console.WriteLine("  11. Salir del programa");
                Console.WriteLine();
                Console.WriteLine("------------------------------------");

                Console.Write("  Digite el Numero de Opción ");
                opcionElegida = Console.ReadLine();

                // Verificamos si la opción es válida (debe ser un número entre 1 y 11)
                if (string.IsNullOrEmpty(opcionElegida) || !int.TryParse(opcionElegida, out int numero) || numero < 1 || numero > 11)
                {
                    Console.WriteLine("  ¡Error! Por favor, elige una opción válida (1-11).");
                    Console.WriteLine("  Presiona ENTER para intentar de nuevo...");
                    Console.ReadLine();
                }
            } while (string.IsNullOrEmpty(opcionElegida) || !int.TryParse(opcionElegida, out int num) || num < 1 || num > 11);

            return opcionElegida;
        }
    }
}