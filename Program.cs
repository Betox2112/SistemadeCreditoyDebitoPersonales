using System;
using System.Collections.Generic;

namespace SistemaDeCreditoYDebitoPersonales
{
    // Clase principal que inicia el programa
    public static class Program
    {
        public static void Main(string[] args)
        {

            // Creamos una instancia de la vista
            Vista.vistaPrincipal vista = new Vista.vistaPrincipal();

            // Creamos una instancia del controlador
            Control.SistemaController controlador = new Control.SistemaController(vista);

            // Iniciamos el programa
            controlador.Iniciar();
        }
    }
}