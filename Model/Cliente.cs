using System;
using System.Collections.Generic;

namespace SistemaDeCreditoYDebitoPersonales.Model
{
    // Clase que representa a un cliente
    public class Cliente
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string TelefonoCelular { get; set; }
        public string TelefonoCasa { get; set; }
        public string TelefonoOficina { get; set; }
        public decimal IngresosMensuales { get; set; }
        public string FuentesIngresos { get; set; }
        public decimal GastosFijosMensuales { get; set; }
        public decimal Ahorros { get; set; }
        public decimal Deuda { get; set; }

        // Método para verificar si el cliente tiene deuda
        public bool TieneDeuda()
        {
            return Deuda > 0;
        }

        // Método para actualizar la deuda del cliente basada en sus préstamos
        public void ActualizarDeuda(List<Prestamo> prestamos)
        {
            Deuda = 0;
            // Usamos un for para recorrer la lista de préstamos
            for (int i = 0; i < prestamos.Count; i++)
            {
                if (prestamos[i].CedulaCliente == Cedula)
                {
                    Deuda += prestamos[i].SaldoPendiente;
                }
            }
        }
    }
}